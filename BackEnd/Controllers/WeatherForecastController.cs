﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Interfaces;
using BackEnd.Models;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WeatherForecastController : BaseController
    {

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly ICuentaService _cuentaService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICuentaService cuentaService)
        {
            _logger = logger;
            _cuentaService = cuentaService;
        }

        [HttpGet]
        public Cuenta Get()
        {
            //Cuenta cuent = _cuentaService.Buscar("123", "123");

            return new Cuenta();
        }
    }
}
