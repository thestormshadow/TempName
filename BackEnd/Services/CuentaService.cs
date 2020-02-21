using BackEnd.Models;
using BackEnd.Services.Interfaces;
using MongoDB.Driver;
using MongoDB.EntitiesManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    public class CuentaService : ICuentaService
    {
        public async Task<bool> existeUsuario(string Correo) => await DB.Find<Cuenta>().Exist(x => x.Status && x.Correo == Correo);

        public async Task<Cuenta> Buscar(string Usuario, string Contraseña)
        {

            return await DB.Find<Cuenta>().OneAsync(x => x.Status && x.Correo == Usuario && x.Contraseña == Contraseña);
        }

        public async Task<Cuenta> Buscar(string ID)
        {
            return await DB.Find<Cuenta>().OneAsync(x => x.Status && x.ID == ID);
        }

        public async Task<Cuenta> Registrar(Cuenta Cuenta)
        {

            if (!await existeUsuario(Cuenta.Correo))
            {
                Cuenta.UltimaConexion = DateTime.Now;
                return await DB.SaveAsync(Cuenta);
            }
            return null;
        }

    }
}
