﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDB.EntitiesManager.Entities
{
    [BsonIgnoreExtraElements]
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// This property is auto managed. Don't ever change this manually.
        /// <para>TIP: If you want to store this entity in a particular database, use the [DatabaseAttribute]</para>
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public DateTime ModifiedOn { get; set; }
        public bool Status
        {
            get; set;
        }
    }
}
