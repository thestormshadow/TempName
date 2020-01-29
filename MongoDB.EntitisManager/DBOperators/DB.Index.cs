using MongoDB.Driver;
using MongoDB.EntitiesManager.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.EntitiesManager
{
    public partial class DB
    {
        internal static async Task CreateIndexAsync<T>(CreateIndexModel<T> model, string db = null)
        {
            await Collection<T>(db).Indexes.CreateOneAsync(model);
        }

        internal static async Task DropIndexAsync<T>(string name, string db = null)
        {
            await Collection<T>(db).Indexes.DropOneAsync(name);
        }

        /// <summary>
        /// Represents an index for a given IEntity
        /// <para>TIP: Define the keys first with .Key() method and finally call the .Create() method.</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public static Index<T> Index<T>(string db = null) where T : IEntity
        {
            return new Index<T>(db);
        }

        /// <summary>
        /// Represents an index for a given IEntity
        /// <para>TIP: Define the keys first with .Key() method and finally call the .Create() method.</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public Index<T> Index<T>() where T : IEntity
        {
            return new Index<T>(DbName);
        }
    }
}
