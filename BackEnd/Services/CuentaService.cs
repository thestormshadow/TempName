using BackEnd.Interfaces;
using BackEnd.Models;
using MongoDB.Driver;
using MongoDB.EntitiesManager;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Services
{
    public class CuentaService : ICuentaService
    {

        public Cuenta getCuenta()
        {         

            var cuenta = (from a in DB.Queryable<Cuenta>()
                          where a.StatusAccount == 1 && a.Usuario == "123" && a.Status == true
                          select a).FirstOrDefault();

            return new Cuenta();
        }

        public List<Cuenta> getCuentas()
        {

            var cuenta = (from a in DB.Queryable<Cuenta>()
                          where a.Status == true
                          select a).ToList();

            return cuenta;
        }

    }
}
