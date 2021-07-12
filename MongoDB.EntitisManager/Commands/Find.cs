using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.EntitiesManager.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDB.EntitiesManager
{
    /// <summary>
    /// Represents a MongoDB Find command for materializing results directly.
    /// <para>TIP: Specify your criteria using .Match() .Sort() .Skip() .Take() .Project() .Option() methods and finally call .Execute()</para>
    /// <para>Note: For building queries, use the DB.Fluent* interfaces</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    public class Find<T> : Find<T, T> where T : IEntity
    {
        internal Find(IClientSessionHandle session = null, string db = null) : base(session, db) { }
    }

    /// <summary>
    /// Represents a MongoDB Find command
    /// <para>TIP: Specify your criteria using .Match() .Sort() .Skip() .Take() .Project() .Option() methods and finally call .Execute()</para>
    /// </summary>
    /// <typeparam name="T">Any class that implements IEntity</typeparam>
    /// <typeparam name="TProjection">The type you'd like to project the results to.</typeparam>
    public class Find<T, TProjection> where T : IEntity
    {
        private FilterDefinition<T> filter = Builders<T>.Filter.Empty;
        private readonly Collection<SortDefinition<T>> sorts = new Collection<SortDefinition<T>>();
        private readonly FindOptions<T, TProjection> options = new FindOptions<T, TProjection>();
        private readonly IClientSessionHandle session = null;
        private readonly string db = null;

        internal Find(IClientSessionHandle session = null, string db = null)
        {
            this.session = session;
            this.db = db;
        }

        /// <summary>
        /// Find a single IEntity by ID
        /// </summary>
        /// <param name="ID">The unique ID of an IEntity</param>
        /// <returns>A single entity or null if not found</returns>
        public TProjection One(string ID)
        {
            return Run.Sync(() => OneAsync(ID));
        }

        /// <summary>
        /// Find a single IEntity by ID
        /// </summary>
        /// <param name="ID">The unique ID of an IEntity</param>
        /// <returns>A single entity or null if not found</returns>
        public async Task<TProjection> OneAsync(string ID)
        {
            Match(ID);
            return (await ExecuteAsync()).SingleOrDefault();
        }

        /// <summary>
        /// Find a single IEntity by ID
        /// </summary>
        /// <param name="ID">The unique ID of an IEntity</param>
        /// <returns>A single entity or null if not found</returns>
        public async Task<TProjection> OneAsync(Expression<Func<T, bool>> expression)
        {
            Match(expression);
            return (await ExecuteAsync()).SingleOrDefault();
        }

        /// <summary>
        /// Find a single IEntity by ID
        /// </summary>
        /// <param name="ID">The unique ID of an IEntity</param>
        /// <returns>A single entity or null if not found</returns>
        public async Task<bool> Exist(Expression<Func<T, bool>> expression)
        {
            Match(expression);
            return ((await ExecuteAsync()).SingleOrDefault() == null) ? false : true;
        }

        /// <summary>
        /// Find entities by supplying a lambda expression
        /// </summary>
        /// <param name="expression">x => x.Property == Value</param>
        /// <returns>A list of Entities</returns>
        public List<TProjection> Many(Expression<Func<T, bool>> expression)
        {
            return Run.Sync(() => ManyAsync(expression));
        }

        /// <summary>
        /// Find entities by supplying a lambda expression
        /// </summary>
        /// <param name="expression">x => x.Property == Value</param>
        /// <returns>A list of Entities</returns>
        public Task<List<TProjection>> ManyAsync(Expression<Func<T, bool>> expression)
        {
            Match(expression);
            return ExecuteAsync();
        }

        /// <summary>
        /// Find entities by supplying a filter expression
        /// </summary>
        /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
        /// <returns>A list of Entities</returns>
        public List<TProjection> Many(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter)
        {
            return Run.Sync(() => ManyAsync(filter));
        }

        /// <summary>
        /// Find entities by supplying a filter expression
        /// </summary>
        /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
        /// <returns>A list of Entities</returns>
        public Task<List<TProjection>> ManyAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter)
        {
            Match(filter);
            return ExecuteAsync();
        }

        /// <summary>
        /// Specify an IEntity ID as the matching criteria
        /// </summary>
        /// <param name="ID">A unique IEntity ID</param>
        public Find<T, TProjection> Match(string ID)
        {
            return Match(f => f.Eq(t => t.ID, ID));
        }

        /// <summary>
        /// Specify the matching criteria with a lambda expression
        /// </summary>
        /// <param name="expression">x => x.Property == Value</param>
        public Find<T, TProjection> Match(Expression<Func<T, bool>> expression)
        {
            return Match(f => f.Where(expression));
        }

        /// <summary>
        /// Specify the matching criteria with a filter expression
        /// </summary>
        /// <param name="filter">f => f.Eq(x => x.Prop, Value) &amp; f.Gt(x => x.Prop, Value)</param>
        public Find<T, TProjection> Match(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter)
        {
            this.filter = filter(Builders<T>.Filter);
            return this;
        }

        /// <summary>
        /// Specify a seach term to find results from the text index of this paricular collection.
        /// <para>TIP: Make sure to define a text index with DB.Index&lt;T&gt;() before searching</para>
        /// </summary>
        /// <param name="searchType">The type of text matching to do</param>
        /// <param name="searchTerm">The search term</param>
        /// <param name="caseSensitive">Case sensitivity of the search (optional)</param>
        /// <param name="diacriticSensitive">Diacritic sensitivity of the search (optional)</param>
        /// <param name="language">The language for the search (optional)</param>
        public Find<T, TProjection> Match(Search searchType, string searchTerm, bool caseSensitive = false, bool diacriticSensitive = false, string language = null)
        {
            if (searchType == Search.Fuzzy)
            {
                searchTerm = searchTerm.ToDoubleMetaphoneHash();
                caseSensitive = false;
                diacriticSensitive = false;
                language = null;
            }

            return Match(
                f => f.Text(
                    searchTerm,
                    new TextSearchOptions
                    {
                        CaseSensitive = caseSensitive,
                        DiacriticSensitive = diacriticSensitive,
                        Language = language
                    }));
        }

        /// <summary>
        /// Specify criteria for matching entities based on GeoSpatial data (longitude &amp; latitude) 
        /// <para>TIP: Make sure to define a Geo2DSphere index with DB.Index&lt;T&gt;() before searching</para>
        /// <para>Note: DB.FluentGeoNear() supports more advanced options</para>
        /// </summary>
        /// <param name="coordinatesProperty">The property where 2DCoordinates are stored</param>
        /// <param name="nearCoordinates">The search point</param>
        /// <param name="maxDistance">Maximum distance in meters from the search point</param>
        /// <param name="minDistance">Minimum distance in meters from the search point</param>
        public Find<T, TProjection> Match(Expression<Func<T, object>> coordinatesProperty, Coordinates2D nearCoordinates, double? maxDistance = null, double? minDistance = null)
        {
            return Match(f => f.Near(coordinatesProperty, nearCoordinates, maxDistance, minDistance)); ;
        }

        /// <summary>
        /// Specify the matching criteria with an aggregation expression (i.e. $expr)
        /// </summary>
        /// <param name="expression">{ $gt: ['$Property1', '$Property2'] }</param>
        public Find<T, TProjection> MatchExpression(string expression)
        {
            filter = "{$expr:" + expression + "}";
            return this;
        }

        /// <summary>
        /// Specify which property and order to use for sorting (use multiple times if needed)
        /// </summary>
        /// <param name="propertyToSortBy">x => x.Prop</param>
        /// <param name="sortOrder">The sort order</param>
        public Find<T, TProjection> Sort(Expression<Func<T, object>> propertyToSortBy, Order sortOrder)
        {
            switch (sortOrder)
            {
                case Order.Ascending:
                    return Sort(s => s.Ascending(propertyToSortBy));

                case Order.Descending:
                    return Sort(s => s.Descending(propertyToSortBy));

                default:
                    return this;
            }
        }

        /// <summary>
        /// Sort the results of a text search by the MetaTextScore
        /// <para>TIP: Use this method after .Project() if you need to do a projection also</para>
        /// </summary>
        public Find<T, TProjection> SortByTextScore()
        {
            return SortByTextScore(null);
        }

        /// <summary>
        /// Sort the results of a text search by the MetaTextScore and get back the score as well
        /// <para>TIP: Use this method after .Project() if you need to do a projection also</para>
        /// </summary>
        /// <param name="scoreProperty">x => x.TextScoreProp</param>
        public Find<T, TProjection> SortByTextScore(Expression<Func<T, object>> scoreProperty)
        {
            switch (scoreProperty)
            {
                case null:
                    AddTxtScoreToProjection("_Text_Match_Score_");
                    return Sort(s => s.MetaTextScore("_Text_Match_Score_"));

                default:
                    AddTxtScoreToProjection(Prop.Dotted(scoreProperty));
                    return Sort(s => s.MetaTextScore(Prop.Dotted(scoreProperty)));
            }
        }

        /// <summary>
        /// Specify how to sort using a sort expression
        /// </summary>
        /// <param name="sortFunction">s => s.Ascending("Prop1").MetaTextScore("Prop2")</param>
        /// <returns></returns>
        public Find<T, TProjection> Sort(Func<SortDefinitionBuilder<T>, SortDefinition<T>> sortFunction)
        {
            sorts.Add(sortFunction(Builders<T>.Sort));
            return this;
        }

        /// <summary>
        /// Specify how many entities to skip
        /// </summary>
        /// <param name="skipCount">The number to skip</param>
        public Find<T, TProjection> Skip(int skipCount)
        {
            options.Skip = skipCount;
            return this;
        }

        /// <summary>
        /// Specify how many entiteis to Take/Limit
        /// </summary>
        /// <param name="takeCount">The number to limit/take</param>
        public Find<T, TProjection> Limit(int takeCount)
        {
            options.Limit = takeCount;
            return this;
        }

        /// <summary>
        /// Specify how to project the results using a lambda expression
        /// </summary>
        /// <param name="expression">x => new Test { PropName = x.Prop }</param>
        public Find<T, TProjection> Project(Expression<Func<T, TProjection>> expression)
        {
            return Project(p => p.Expression(expression));
        }

        /// <summary>
        /// Specify how to project the results using a projection expression
        /// </summary>
        /// <param name="projection">p => p.Include("Prop1").Exclude("Prop2")</param>
        /// <returns></returns>
        public Find<T, TProjection> Project(Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T, TProjection>> projection)
        {
            options.Projection = projection(Builders<T>.Projection);
            return this;
        }

        /// <summary>
        /// Specify an option for this find command (use multiple times if needed)
        /// </summary>
        /// <param name="option">x => x.OptionName = OptionValue</param>
        public Find<T, TProjection> Option(Action<FindOptions<T, TProjection>> option)
        {
            option(options);
            return this;
        }

        /// <summary>
        /// Run the Find command in MongoDB server and get the results
        /// </summary>
        /// <returns>A list of entities</returns>
        public List<TProjection> Execute()
        {
            return Run.Sync(() => ExecuteAsync());
        }

        /// <summary>
        /// Run the Find command in MongoDB server and get the results
        /// </summary>
        /// <returns>A list of entities</returns>
        public Task<List<TProjection>> ExecuteAsync()
        {
            if (sorts.Count > 0) options.Sort = Builders<T>.Sort.Combine(sorts);
            return FillDetails(DB.FindAsync(filter, options, session, db));
        }

        public Task<List<TProjection>> FillDetails(Task<List<TProjection>> liPredictions)
        {
            foreach (TProjection Projection in liPredictions.Result)
            {
                PropertyInfo[] properties = Projection.GetType().GetProperties();

                List<PropertyInfo> IdProperty = (from PropertyInfo property in properties
                                                 where property.GetCustomAttributes(typeof(ForeignField), true).Length > 0
                                                 select property).ToList();

                foreach (PropertyInfo property in IdProperty)
                {
                    if (!property.GetCustomAttribute<ForeignField>().IsFillAutomaticDetails)
                        continue;

                    string FF = property.GetCustomAttribute<ForeignField>().Name;                    
                                        
                    Type type = property.PropertyType;
                    PropertyInfo ID = properties.Where(x => x.Name == "ID").FirstOrDefault();
                    string IDs = ID.GetValue(Projection).ToString();
                    List<dynamic> genList = new List<dynamic>();
                    string TableName = (property.GetCustomAttribute<ForeignField>().TableName == "") ? property.PropertyType.Name + "s" : property.GetCustomAttribute<ForeignField>().TableName;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        TableName = (property.GetCustomAttribute<ForeignField>().TableName == "") ? type.GetGenericArguments()[0].Name + "s" : property.GetCustomAttribute<ForeignField>().TableName;
                        genList = DB.CollectionName<dynamic>(TableName)
                            .Find("{" + FF + ": '" + IDs + "', Status : true }").ToList();
                    }
                    else
                        genList = DB.CollectionName<dynamic>(TableName)
                                .Find("{" + FF + ": '" + IDs + "', Status : true }").ToList();
                    
                    FillObject(type, genList, property, Projection);
                }
            }
            return liPredictions;
        }

        public void FillObject(Type type, List<dynamic> genList, PropertyInfo property,  TProjection Projection)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))            
                FillDynamicList(type, genList, property, Projection);            
            else if (genList.Count == 1)
                FillDynamicObject(genList, property, Projection);
            else if (genList.Count > 0)
                throw new Exception("[Error] Mas de un resultado encontrado en el foreignObject durante el FillDetails");
        }        
        public void FillDynamicList(Type type, List<dynamic> genList, PropertyInfo property, TProjection Projection)
        {
            Type ListType = type.GetGenericArguments()[0];
            System.Collections.IList lista = CreateList(ListType);
            foreach (dynamic objD in genList)
            {                
                var values = (IDictionary<string, object>)objD;
                Object obj = Activator.CreateInstance(ListType);
                
                lista.Add(FillDynaimcProperty(obj,values));
            }
            property.SetValue(Projection, lista);
        }
        public void FillDynamicObject(List<dynamic> genList, PropertyInfo property, TProjection Projection)
        {
            var values = (IDictionary<string, object>)genList[0];
            Object obj = Activator.CreateInstance(property.PropertyType);
            property.SetValue(Projection, FillDynaimcProperty(obj, values));
        }
        public Object FillDynaimcProperty(Object obj, IDictionary<string, object> values)
        {
            Type t = obj.GetType();
            List<PropertyInfo> props = t.GetProperties().ToList();
            foreach (PropertyInfo pI in props)
            {
                if (pI.PropertyType.Namespace != "BackEnd.Models")
                {
                    if (pI.Name == "ID")
                        pI.SetValue(obj, values.Where(x => x.Key == "_id").Select(x => x.Value).SingleOrDefault().ToString());
                    else
                        pI.SetValue(obj, values.Where(x => x.Key == pI.Name).Select(x => x.Value).SingleOrDefault());
                }
                else
                    pI.SetValue(obj, null);
            }
            return obj;
        }
        
        public System.Collections.IList CreateList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (System.Collections.IList)Activator.CreateInstance(genericListType);
        }

        private void AddTxtScoreToProjection(string propName)
        {
            if (options.Projection == null) options.Projection = "{}";

            options.Projection =
                options.Projection
                .Render(BsonSerializer.SerializerRegistry.GetSerializer<T>(), BsonSerializer.SerializerRegistry)
                .Document.Add(propName, new BsonDocument { { "$meta", "textScore" } });
        }
    }

    public enum Order
    {
        Ascending,
        Descending
    }

    public enum Search
    {
        Fuzzy,
        Full
    }
}
