using BackEnd.Models;
using BackEnd.Services.Interfaces;
using MongoDB.Driver;
using MongoDB.EntitiesManager;
using System;
using System.Linq;

namespace BackEnd.Services
{
    public class CuentaService : ICuentaService
    {
        public bool existeUsuario(string Correo)
        {
            var cuentas = (from a in DB.Queryable<Cuenta>()
                           where a.Correo == Correo
                           select a).ToList();

            return (cuentas.Count > 0) ? true : false;
        }

        public Cuenta Buscar(string Usuario, string Contraseña)
        {
            var cuenta = (from a in DB.Queryable<Cuenta>()
                          join o in DB.Queryable<Usuario>() on a.ID equals o.ID_Cuenta into UsuarioObj
                          where a.Correo == Usuario && a.Contraseña == Contraseña && a.Status == true
                          select new Cuenta()
                          {
                              ID = a.ID,
                              Correo = a.Correo,
                              Contraseña = a.Contraseña,
                              InfoUsuario = UsuarioObj.FirstOrDefault(),
                              StatusAccount = a.StatusAccount,
                              Status = a.Status,
                              ModifiedOn = a.ModifiedOn,
                              UltimaConexion = a.UltimaConexion
                          }).FirstOrDefault();

            return cuenta;
        }

        public Cuenta Buscar(string ID)
        {
            return (from a in DB.Queryable<Cuenta>() where a.Status == true select a).FirstOrDefault();
        }

        public Cuenta Registrar(Cuenta Cuenta)
        {

            if (!existeUsuario(Cuenta.Correo))
            {
                Cuenta.UltimaConexion = DateTime.Now;
                DB.Save(Cuenta);
                return Cuenta;
            }
            return null;
        }

    }
}
