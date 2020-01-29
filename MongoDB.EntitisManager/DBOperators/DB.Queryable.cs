﻿using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDB.EntitiesManager
{
    public partial class DB
    {
        /// <summary>
        /// Exposes the MongoDB collection for the given IEntity as an IQueryable in order to facilitate LINQ queries.
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public static IMongoQueryable<T> Queryable<T>(AggregateOptions options = null, string db = null)
        {
            return Collection<T>(db).AsQueryable(options);
        }

        /// <summary>
        /// Exposes the MongoDB collection for the given IEntity as an IQueryable in order to facilitate LINQ queries.
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public IMongoQueryable<T> Queryable<T>(AggregateOptions options = null)
        {
            return Queryable<T>(options, DbName);
        }
    }
}
