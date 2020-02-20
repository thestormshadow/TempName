using BackEnd.Models;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(Cuenta cuenta)
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

        public ClaimsPrincipal GetTokenClaims(string token)
        {
            if (String.IsNullOrEmpty(token))
                throw new ArgumentException("null or Empty Token");

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
            ClaimsPrincipal claimsPrincipal = null;
            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception e)
            {
                string err = e.Message;
                return claimsPrincipal;
            }

            return claimsPrincipal;
        }

        public bool IsValidToken(string token)
        {

            if (String.IsNullOrEmpty(token))
                return false;

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
