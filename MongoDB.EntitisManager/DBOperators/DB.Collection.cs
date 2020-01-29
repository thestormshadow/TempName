using MongoDB.Driver;
using System;
using System.Reflection;

namespace MongoDB.EntitiesManager
{
    public partial class DB
    {
        /// <summary>
        /// Gets the IMongoCollection for a given IEntity type.
        /// <para>TIP: Try never to use this unless really neccessary.</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public static IMongoCollection<T> Collection<T>(string db = null)
        {
            return GetDB(db).GetCollection<T>(GetCollectionName<T>());
        }

        /// <summary>
        /// Gets the IMongoCollection for a given IEntity type.
        /// <para>TIP: Try never to use this unless really neccessary.</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public IMongoCollection<T> Collection<T>()
        {
            return Collection<T>(DbName);
        }

        internal static string GetCollectionName<T>()
        {
            string collection = typeof(T).Name;

            var attribute = typeof(T).GetTypeInfo().GetCustomAttribute<CollectionName>();
            if (attribute != null)
            {
                collection = attribute.Name;
            }

            if (string.IsNullOrWhiteSpace(collection) || collection.Contains("~")) throw new ArgumentException("This is an illegal name for a collection!");

            return collection;
        }

        //internal static IMongoCollection<JoinRecord> GetRefCollection(string name, string db = null)
        //{
        //    return GetDB(db).GetCollection<JoinRecord>(name);
        //}
    }
}
