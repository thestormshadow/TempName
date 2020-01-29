﻿using MongoDB.Driver;
using MongoDB.EntitiesManager.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.EntitiesManager
{
    public partial class DB
    {
        internal static async Task<List<TProjection>> FindAsync<T, TProjection>(FilterDefinition<T> filter, FindOptions<T, TProjection> options, IClientSessionHandle session = null, string db = null)
        {
            return await (session == null
                ? (await Collection<T>(db).FindAsync(filter, options)).ToListAsync()
                : (await Collection<T>(db).FindAsync(session, filter, options)).ToListAsync());
        }

        /// <summary>
        /// Represents a MongoDB Find command
        /// <para>TIP: Specify your criteria using .Match() .Sort() .Skip() .Take() .Project() .Option() methods and finally call .Execute()</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public static Find<T> Find<T>(string db = null) where T : IEntity
        {
            return new Find<T>(db: db);
        }

        /// <summary>
        /// Represents a MongoDB Find command
        /// <para>TIP: Specify your criteria using .Match() .Sort() .Skip() .Take() .Project() .Option() methods and finally call .Execute()</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        public Find<T> Find<T>() where T : IEntity
        {
            return new Find<T>(db: DbName);
        }

        /// <summary>
        /// Represents a MongoDB Find command
        /// <para>TIP: Specify your criteria using .Match() .Sort() .Skip() .Take() .Project() .Option() methods and finally call .Execute()</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        /// <typeparam name="TProjection">The type that is returned by projection</typeparam>
        /// <returns></returns>
        public static Find<T, TProjection> Find<T, TProjection>(string db = null) where T : IEntity
        {
            return new Find<T, TProjection>(db: db);
        }

        /// <summary>
        /// Represents a MongoDB Find command
        /// <para>TIP: Specify your criteria using .Match() .Sort() .Skip() .Take() .Project() .Option() methods and finally call .Execute()</para>
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        /// <typeparam name="TProjection">The type that is returned by projection</typeparam>
        /// <returns></returns>
        public Find<T, TProjection> Find<T, TProjection>() where T : IEntity
        {
            return new Find<T, TProjection>(db: DbName);
        }
    }
}
