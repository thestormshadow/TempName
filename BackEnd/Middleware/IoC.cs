using BackEnd.Interfaces;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Middleware
{
    public static class IoC
    {
        public static IServiceCollection addRegistration(this IServiceCollection services)
        {
            services.AddSingleton<ICuentaService, CuentaService>();
            services.AddSingleton<IUsuarioService, UsuarioService>();
            return services;
        }
    }
}
