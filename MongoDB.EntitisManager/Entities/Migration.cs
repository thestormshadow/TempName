using MongoDB.EntitiesManager.Entities;

namespace MongoDB.EntitiesManager
{
    [CollectionName("_migration_history_")]
    public class Migration : Entity
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public double TimeTakenSeconds { get; set; }
    }
}
