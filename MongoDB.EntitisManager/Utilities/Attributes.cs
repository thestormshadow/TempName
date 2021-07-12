using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDB.EntitiesManager
{
    /// <summary>
    /// Indicates that this property should be ignored when this class is persisted to MongoDB.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : BsonIgnoreAttribute { }

    /// <summary>
    /// Use this attribute to specify a custom MongoDB collection name for an IEntity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionName : Attribute
    {
        public string Name { get; }

        /// <param name="name">The name you want to use for the collection</param>
        public CollectionName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Name = name;
        }
    }

    /// <summary>
    /// Use this attribute to specify the database to store this IEntity in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseAttribute : Attribute
    {
        public string Name { get; }

        /// <param name="name">The name you want to use for the collection</param>
        public DatabaseAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Name = name;
        }
    }

    /// <summary>
    /// Use this attribute to specify the database to store this IEntity in.
    /// </summary>
    public class ForeignField : Attribute
    {
        public string Name { get; }
        public string TableName { get; }
        public bool IsFillAutomaticDetails { get; }

        /// <param name="name">The name you want to use for the collection</param>
        public ForeignField(string name, string tableName = "", bool isFillAutomaticDetails = false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Name empty");
            Name = name;
            TableName = tableName;
            IsFillAutomaticDetails = isFillAutomaticDetails;
        }
    }
}
