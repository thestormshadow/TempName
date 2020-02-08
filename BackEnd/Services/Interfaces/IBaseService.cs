using MongoDB.EntitiesManager;
using MongoDB.EntitiesManager.Entities;
using System.Collections.Generic;

namespace BackEnd.Interfaces
{
    interface IBaseService
    {
        T Guardar<T>(T Obj) where T : Entity;
        T Buscar<T>(string Id) where T : Entity;
        List<T> Listar<T>() where T : Entity;
    }
}
