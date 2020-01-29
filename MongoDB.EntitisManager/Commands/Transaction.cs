﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntitiesManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.EntitiesManager
{
    /// <summary>
    /// Represents a transaction used to carry out inter-related write operations.
    /// <para>TIP: Remember to always call .Dispose() after use or enclose in a 'Using' statement.</para>
    /// <para>IMPORTANT: Use the methods on this transaction to perform operations and not the methods on the DB class.</para>
    /// </summary>
    public class Transaction : IDisposable
    {
        public IClientSessionHandle Session { get; }
        private readonly IMongoClient client;
        private readonly string db = null;

        /// <summary>
        /// Instantiates and begins a transaction.
        /// </summary>
        /// <param name="database">The name of the database to use for this transaction</param>
        /// <param name="options">Client session options for this transaction</param>
        public Transaction(string database = null, ClientSessionOptions options = null)
        {
            db = database;
            client = DB.GetClient(db);
            Session = client.StartSession(options);
            Session.StartTransaction();
        }

        /// <summary>
        /// Commits a tranaction to MongoDB
        /// </summary>
        public void Commit() => Session.CommitTransaction();

        /// <summary>
        /// Commits a tranaction to MongoDB
        /// </summary>
        public async Task CommitAsync() => await Session.CommitTransactionAsync();

        /// <summary>
        /// Aborts and rolls back a tranaction
        /// </summary>
        public void Abort() => Session.AbortTransaction();

        /// <summary>
        /// Aborts and rolls back a tranaction
        /// </summary>
        public async Task AbortAsync() => await Session.AbortTransactionAsync();

        public Update<T> Update<T>() where T : IEntity
        {
            return new Update<T>(Session, db);
        }

        public Find<T> Find<T>() where T : IEntity
        {
            return new Find<T>(Session, db);
        }

        public Find<T, TProjection> Find<T, TProjection>() where T : IEntity
        {
            return new Find<T, TProjection>(Session, db);
        }

        public IAggregateFluent<T> Fluent<T>(AggregateOptions options = null)
        {
            return DB.Fluent<T>(options, Session, db);
        }

        public IAggregateFluent<T> GeoNear<T>(Coordinates2D NearCoordinates, Expression<Func<T, object>> DistanceField, bool Spherical = true, int? MaxDistance = null, int? MinDistance = null, int? Limit = null, BsonDocument Query = null, int? DistanceMultiplier = null, Expression<Func<T, object>> IncludeLocations = null, string IndexKey = null, AggregateOptions options = null) where T : IEntity
        {
            return DB.FluentGeoNear(NearCoordinates, DistanceField, Spherical, MaxDistance, MinDistance, Limit, Query, DistanceMultiplier, IncludeLocations, IndexKey, options, Session, db);
        }

        public void Save<T>(T entity) where T : IEntity
        {
            Run.Sync(() => SaveAsync(entity));
        }

        public async Task SaveAsync<T>(T entity) where T : IEntity
        {
            await DB.SaveAsync(entity, Session, db);
        }

        public void Save<T>(IEnumerable<T> entities) where T : IEntity
        {
            Run.Sync(() => SaveAsync(entities));
        }

        public async Task SaveAsync<T>(IEnumerable<T> entities) where T : IEntity
        {
            await DB.SaveAsync(entities, Session, db);
        }

        public void Delete<T>(string ID) where T : IEntity
        {
            Run.Sync(() => DeleteAsync<T>(ID));
        }

        public async Task DeleteAsync<T>(string ID) where T : IEntity
        {
            await DB.DeleteAsync<T>(ID, Session, db);
        }

        public void Delete<T>(Expression<Func<T, bool>> expression) where T : IEntity
        {
            Run.Sync(() => DeleteAsync(expression));
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> expression) where T : IEntity
        {
            await DB.DeleteAsync(expression, Session, db);
        }

        public void Delete<T>(IEnumerable<string> IDs) where T : IEntity
        {
            Run.Sync(() => DeleteAsync<T>(IDs));
        }

        public async Task DeleteAsync<T>(IEnumerable<string> IDs) where T : IEntity
        {
            await DB.DeleteAsync<T>(IDs, Session, db);
        }

        public IAggregateFluent<T> FluentTextSearch<T>(Search searchType, string searchTerm, bool caseSensitive = false, bool diacriticSensitive = false, string language = null, AggregateOptions options = null)
        {
            return DB.FluentTextSearch<T>(searchType, searchTerm, caseSensitive, diacriticSensitive, language, options, Session, db);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Session.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion        
    }
}
