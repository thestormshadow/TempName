using BackEnd.Models;
using BackEnd.Services.Interfaces;
using MongoDB.EntitiesManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    public class UsuarioService : IUsuarioService
    {
        //private readonly ICuentaService _cuentaService;
        
        public Usuario Buscar(string ID)
        {
            var usuario = (from a in DB.Queryable<Usuario>()
                          where a.ID == ID && a.Status == true
                          select a).FirstOrDefault();

            return usuario;
        }

        public Usuario Buscar_PorIdCuenta(string IDCuenta)
        {
            Usuario response = null;

            if (IDCuenta != null || IDCuenta != String.Empty)
                response = (from a in DB.Queryable<Usuario>() where a.ID_Cuenta == IDCuenta select a).FirstOrDefault();

            return response;
        }

        public List<Usuario> Listar() => (from a in DB.Queryable<Usuario>() select a).ToList();

        public List<Usuario> Listar_PorNombre(string Nombre) => (from a in DB.Queryable<Usuario>() where a.Nombres == Nombre select a).ToList();

        public List<Usuario> Listar_PorNombreCompleto(string NombreCompleto) => (from a in DB.Queryable<Usuario>() where (a.Nombres + a.ApellidoP + a.ApellidoM).Contains(NombreCompleto) select a).ToList();

        public Usuario Registrar(Usuario usuario)
        {
            DB.Save(usuario);
            return usuario;
        }
    }
}
