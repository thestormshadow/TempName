using MongoDB.EntitiesManager;
using MongoDB.EntitiesManager.Entities;
using System;

namespace BackEnd.Models
{
    [CollectionName("Usuarios")]
    public class Usuario : Entity
    {
        public string Nombres { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public int Edad { get; set; }
    }
}
