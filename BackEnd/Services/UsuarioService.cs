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

        public async Task<Usuario> Buscar(string ID) => await DB.Find<Usuario>().OneAsync(x => x.Status && x.ID == ID);

        public async Task<Usuario> Buscar_PorIdCuenta(string IDCuenta) => await DB.Find<Usuario>().OneAsync(x => x.Status && x.ID_Cuenta == IDCuenta);

        public async Task<List<Usuario>> Listar() => await DB.Find<Usuario>().ManyAsync(x => x.Status);

        public async Task<List<Usuario>> Listar_PorNombre(string Nombre) => await DB.Find<Usuario>().ManyAsync(x => x.Status && x.Nombres == Nombre);

        public async Task<List<Usuario>> Listar_PorNombreCompleto(string NombreCompleto) => await DB.Find<Usuario>().ManyAsync(x => x.Status && (x.Nombres + " " + x.ApellidoP + " " + x.ApellidoM).Contains(NombreCompleto));

        public async Task<Usuario> Registrar(Usuario usuario) => await DB.SaveAsync(usuario);
    }
}
