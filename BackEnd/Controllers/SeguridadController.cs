using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.EntitiesManager;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguridadController : BaseController
    {
        private readonly ICuentaService _cuentaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;        

        public SeguridadController(ICuentaService cuentaService, IUsuarioService usuarioService, IAuthService authService)
        {
            _cuentaService = cuentaService;
            _usuarioService = usuarioService;
            _authService = authService;
            
        }

        // GET: api/Seguridad
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("accestoken")]
        public async Task<ActionResult> LoginWithToken([FromBody] OnlyToken token)
        {            
            if (_authService.IsValidToken(token.Token))
            {                
                Usuario usuario = await _usuarioService.Buscar_PorIdCuenta(_authService.GetTokenClaims(token.Token)?.FindFirst("IDCuenta")?.Value);
                return Ok(new { access_token = token.Token, user = usuario });
            }

            return BadRequest();
        }
        
        // POST: api/Seguridad/login
        [HttpPost("login")]
        public async Task<ActionResult> Post([FromBody] Login obj)
        {
            Cuenta result = await _cuentaService.Buscar(obj.Correo, obj.Password);
            
            if (result == null)
                return NotFound(obj);
            else
            {
                Usuario usuario = await _usuarioService.Buscar_PorIdCuenta(result.ID);
                return Ok(new { access_token = _authService.GenerateToken(result), user = usuario });
            }            
        }
                
        // POST: api/Seguridad/register/{key}
        [HttpPost("register")]
        public async Task<ActionResult> Post([FromBody] Register obj)
        {
            try
            {
                Cuenta _cuenta = obj.Cuenta;
                Usuario _usuario = obj.Usuario;

                if (!await _cuentaService.existeUsuario(_cuenta.Correo))
                {
                    _cuenta = await _cuentaService.Registrar(_cuenta);
                    if (_cuenta != null)
                    {
                        _usuario.ID_Cuenta = _cuenta.ID;
                        _usuario = await _usuarioService.Registrar(_usuario);
                        _cuenta.InfoUsuario = _usuario;
                        DB.Save(_cuenta);
                        return Ok(new { access_token = _authService.GenerateToken(_cuenta), user = _usuario });
                    }
                }
                else
                {
                    return BadRequest(new { Error = "Ya existe el correo" });
                }

                return BadRequest(new { Error = "Error al registrar la cuenta" });
            }
            catch (Exception error)
            {
                return Problem(error.Message,null,error.HResult,error.Message,error.GetType().ToString());               
            }            
        }

        // PUT: api/Seguridad/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }    

    }
}
