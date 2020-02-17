using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.EntitiesManager;


namespace BackEnd.Controllers
{
    public partial class BaseController : ControllerBase
    {
        public BaseController()
        {
        }        
    }
}