using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IAuthService
    {
        bool IsValidToken(string token);
        string GenerateToken(Cuenta cuenta);
        ClaimsPrincipal GetTokenClaims(string token);
    }
}
