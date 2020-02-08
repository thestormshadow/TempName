using BackEnd.Interfaces;
using MongoDB.EntitiesManager;
using MongoDB.EntitiesManager.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Services
{
    public partial class BaseService : IBaseService
    {
        public virtual T Guardar<T>(T Obj) where T : Entity
        {
            DB.Save(Obj);
            return Obj;
        }

        public T Buscar<T>(string Id) where T : Entity
        {
            return (from a in DB.Queryable<T>()
                    where a.ID == Id && a.Status == true
                    select a).FirstOrDefault();
        }

        public List<T> Listar<T>() where T : Entity
        {
            return (from a in DB.Queryable<T>()
                    where a.Status == true
                    select a).ToList(); 
        }
    }
}
