using MongoDB.EntitiesManager;
using MongoDB.EntitiesManager.Entities;
using System;
using System.Security;

namespace BackEnd.Models
{
    [CollectionName("Cuentas")]
    public class Cuenta : Entity
    {
        private string contraseña;
        private string correo;

        public string Correo { get => correo; set => correo = SecurityElement.Escape(value); }
        public string Contraseña { get => contraseña; set => contraseña = SecurityElement.Escape(value); }
        public Usuario InfoUsuario { get; set; }
        public DateTime UltimaConexion { get; set; }
        public int StatusAccount { get; set; }
    }
}
