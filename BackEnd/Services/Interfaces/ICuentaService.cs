using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface ICuentaService
    { 
        bool existeUsuario(string Usuario);
        Cuenta Buscar(string Usuario, string Contraseña);
        Cuenta Buscar(string ID);
        public Cuenta Registrar(Cuenta Cuenta);
    }
}
