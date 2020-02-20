using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> Registrar(Usuario usuario);
        Task<Usuario> Buscar(string ID);
        Task<List<Usuario>> Listar();
        Task<Usuario> Buscar_PorIdCuenta(string IDCuenta);
        Task<List<Usuario>> Listar_PorNombre(string Nombre);
        Task<List<Usuario>> Listar_PorNombreCompleto(string NombreCompleto);

    }
}
