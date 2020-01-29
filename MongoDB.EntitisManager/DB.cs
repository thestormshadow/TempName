using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.EntitiesManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.EntitiesManager
{
    public partial class DB
    {
        #region Variables
        /// <summary>
        /// Obtiene el nombre de la DataBase de la instancia
        /// </summary>
        public string DbName { get; private set; } = null;

        private static readonly Dictionary<string, IMongoDatabase> dbs = new Dictionary<string, IMongoDatabase>();
        private static readonly Dictionary<string, DB> instances = new Dictionary<string, DB>();
        private static bool isSetupDone = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the MongoDB connection with the given connection parameters.
        /// </summary>
        /// <param name="database">Name of the database</param>
        /// <param name="host">Adderss of the MongoDB server</param>
        /// <param name="port">Port number of the server</param>
        public DB(string database, string host = "127.0.0.1", int port = 27017)
        {
            Initialize(
                new MongoClientSettings { Server = new MongoServerAddress(host, port) }, database);
        }

        /// <summary>
        /// Initializes the MongoDB connection with an advanced set of parameters.
        /// </summary>
        /// <param name="settings">A MongoClientSettings object</param>
        /// <param name="database">Name of the database</param>
        public DB(MongoClientSettings settings, string database)
        {
            Initialize(settings, database);
        }

        /// <summary>
        /// Initializes the MongoDB connection with an advanced set of parameters.
        /// </summary>
        /// <param name="connectionString">A string Database</param>
        /// <param name="database">Name of the database</param>
        public DB(string connectionString, string database)
        {
            Initialize(connectionString, database);
        }
        #endregion

        #region Inicializadores

        private void Initialize(MongoClientSettings settings, string db)
        {
            if (string.IsNullOrEmpty(db)) throw new ArgumentNullException("database", "Database name cannot be empty!");

            DbName = db;

            if (dbs.ContainsKey(db)) return;

            try
            {
                dbs.Add(db, new MongoClient(settings).GetDatabase(db));
                instances.Add(db, this);
                dbs[db].ListCollectionNames().ToList(); //get the list of collection names so that first db connection is established
            }
            catch (Exception)
            {
                dbs.Remove(db);
                instances.Remove(db);
                DbName = null;
                throw;
            }

            if (!isSetupDone)
            {
                BsonSerializer.RegisterSerializer(new ByteSerializer());
                BsonSerializer.RegisterSerializer(new FuzzyStringSerializer());
                BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
                BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));

                ConventionRegistry.Register(
                    "IgnoreExtraElements",
                    new ConventionPack { new IgnoreExtraElementsConvention(true) },
                    type => true);

                ConventionRegistry.Register(
                    "IgnoreManyProperties",
                    new ConventionPack { new IgnoreManyPropertiesConvention() },
                    type => true);

                isSetupDone = true;
            }
        }

        private void Initialize(string connectionString, string db)
        {
            if (string.IsNullOrEmpty(db)) throw new ArgumentNullException("database", "Database name cannot be empty!");

            DbName = db;

            if (dbs.ContainsKey(db)) return;

            try
            {
                dbs.Add(db, new MongoClient(connectionString).GetDatabase(db));
                instances.Add(db, this);
                dbs[db].ListCollectionNames().ToList(); //get the list of collection names so that first db connection is established
            }
            catch (Exception)
            {
                dbs.Remove(db);
                instances.Remove(db);
                DbName = null;
                throw;
            }

            if (!isSetupDone)
            {
                BsonSerializer.RegisterSerializer(new DateSerializer());
                BsonSerializer.RegisterSerializer(new FuzzyStringSerializer());
                BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
                BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));

                ConventionRegistry.Register(
                    "IgnoreExtraElements",
                    new ConventionPack { new IgnoreExtraElementsConvention(true) },
                    type => true);

                ConventionRegistry.Register(
                    "IgnoreManyProperties",
                    new ConventionPack { new IgnoreManyPropertiesConvention() },
                    type => true);

                isSetupDone = true;
            }
        }

        #endregion

        #region Methods

        internal class IgnoreManyPropertiesConvention : ConventionBase, IMemberMapConvention
        {
            public void Apply(BsonMemberMap mMap)
            {
                if (mMap.MemberType.Name == "Many`1")
                {
                    mMap.SetShouldSerializeMethod(o => false);
                }
            }
        }

        private static IMongoDatabase GetDB(string database)
        {
            IMongoDatabase db = null;

            if (dbs.Count > 0)
            {
                if (string.IsNullOrEmpty(database))
                {
                    db = dbs.First().Value;
                }
                else
                {
                    dbs.TryGetValue(database, out db);
                }
            }

            if (db == null) throw new InvalidOperationException($"Database connection is not initialized for [{database}]");

            return db;
        }
                     
        internal static IMongoClient GetClient(string db = null)
        {
            return GetDB(db).Client;
        }

        /// <summary>
        /// Returns the DB instance for a given database name.
        /// </summary>
        /// <param name="database">The database name to retrieve the DB instance for</param>
        /// <exception cref="InvalidOperationException">Throws an exeception if the database has not yet been initialized</exception>
        public static DB GetInstance(string database)
        {
            if (instances.ContainsKey(database)) return instances[database];

            throw new InvalidOperationException($"An instance has not been initialized yet for [{database}]");
        }

        /// <summary>
        /// Returns a new instance of the supplied IEntity type
        /// </summary>
        /// <typeparam name="T">Any class that implements IEntity</typeparam>
        /// <returns></returns>
        public static T Entity<T>() where T : IEntity, new()
        {
            return new T();
        }

        #endregion
    }
}
