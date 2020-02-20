using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface ICuentaService
    { 
        Task<bool> existeUsuario(string Usuario);
        Task<Cuenta> Buscar(string Usuario, string Contraseña);
        Task<Cuenta> Buscar(string ID);
        Task<Cuenta> Registrar(Cuenta Cuenta);
    }
}
