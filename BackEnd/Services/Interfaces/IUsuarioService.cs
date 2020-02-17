using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IUsuarioService
    {
        Usuario Registrar(Usuario usuario);
        Usuario Buscar(string ID);
        List<Usuario> Listar();
        Usuario Buscar_PorIdCuenta(string IDCuenta);
        List<Usuario> Listar_PorNombre(string Nombre);
        List<Usuario> Listar_PorNombreCompleto(string NombreCompleto);

    }
}
