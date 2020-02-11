using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using BackEnd.Models;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.EntitiesManager;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguridadController : BaseController
    {
        private readonly ICuentaService _cuentaService;
        private readonly IUsuarioService _usuarioService;
        public SeguridadController(ICuentaService cuentaService, IUsuarioService usuarioService)
        {
            _cuentaService = cuentaService;
            _usuarioService = usuarioService;
        }

        // GET: api/Seguridad
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // POST: api/Seguridad/login
        [HttpPost("login")]
        public ActionResult Post([FromBody] Login obj)
        {
            Cuenta result = _cuentaService.Buscar(obj.Usuario, obj.Password);
            if (result == null)
                return NotFound(obj);
            
            return Ok(result);
        }

        // POST: api/Seguridad/register/{key}
        [HttpPost("register")]
        public ActionResult Post([FromBody] Register obj)
        {
            Cuenta _cuenta = obj.Cuenta;
            Usuario _usuario = obj.Usuario;

            if (!_cuentaService.existeUsuario(_cuenta.Correo))
            {
                _cuenta = _cuentaService.Registrar(_cuenta);
                if (_cuenta != null)
                {
                    _usuario.ID_Cuenta = _cuenta.ID;
                    _usuario = _usuarioService.Registrar(_usuario);
                    _cuenta.InfoUsuario = _usuario;
                    DB.Save(_cuenta);
                    return Ok(_cuenta);
                }
            }
            else
            {
                return BadRequest(new { Error = "Ya existe el correo" });
            }            
            return BadRequest(new { Error = "Error al registrar la cuenta" });
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
