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
        private readonly IConfiguration _configuration;

        public SeguridadController(ICuentaService cuentaService, IUsuarioService usuarioService, IConfiguration configuration)
        {
            _cuentaService = cuentaService;
            _usuarioService = usuarioService;
            _configuration = configuration;
            
        }

        // GET: api/Seguridad
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("accestoken")]
        public ActionResult LoginWithToken([FromBody] OnlyToken token)
        {
            
            if (ValidateToken(token.Token))
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);
                Usuario usuario = _usuarioService.Buscar_PorIdCuenta(jwt.Claims.Where(x => x.Type == "IDCuenta").ToList().FirstOrDefault().Value);
                return Ok(new { access_token = token.Token, user = usuario });
            }

            return BadRequest();
        }
        
        // POST: api/Seguridad/login
        [HttpPost("login")]
        public ActionResult Post([FromBody] Login obj)
        {
            Cuenta result = _cuentaService.Buscar(obj.Correo, obj.Password);
            
            if (result == null)
                return NotFound(obj);
            else
            {
                Usuario usuario = _usuarioService.Buscar_PorIdCuenta(result.ID);
                return Ok(new { access_token = BuildToken(result), user = usuario });
            }            
        }
                
        // POST: api/Seguridad/register/{key}
        [HttpPost("register")]
        public ActionResult Post([FromBody] Register obj)
        {
            try
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
                        return Ok(new { access_token = BuildToken(_cuenta), user = _usuario });
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

        private string BuildToken(Cuenta cuenta)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, cuenta.Correo),
                new Claim("IDCuenta",cuenta.ID),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "https://my-issuer.com/trust/issuer",
                audience: "https://my-rp.com",
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = creds.Key,
                ValidateAudience = false,
                ValidIssuer = "https://my-issuer.com/trust/issuer",
                RequireExpirationTime = true
            };

            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception e)
            {
                string err = e.Message;
                return false;
            }

            return validatedToken != null;
        }

    }
}
