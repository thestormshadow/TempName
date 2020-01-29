using MongoDB.EntitiesManager;
using MongoDB.EntitiesManager.Entities;
using System;

namespace BackEnd.Models
{
    [CollectionName("Cuenta")]
    public class Cuenta : Entity
    {
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public Usuario InfoUsuario { get; set; }
        public DateTime UltimaConexion { get; set; }
        public int StatusAccount { get; set; }
    }
}
