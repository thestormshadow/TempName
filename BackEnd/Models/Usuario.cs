using MongoDB.EntitiesManager;
using MongoDB.EntitiesManager.Entities;
using System;

namespace BackEnd.Models
{
    [CollectionName("Usuarios")]
    public class Usuario : Entity
    {
        private string nombres;
        private string apellidoP;
        private string apellidoM;

        public string ID_Cuenta { get; set; }
        public string Nombres { get => nombres; set => nombres = value.Trim(); }
        public string ApellidoP { get => apellidoP; set => apellidoP = value.Trim(); }
        public string ApellidoM { get => apellidoM; set => apellidoM = value.Trim(); }
        public int Edad { get; set; }
    }
}
