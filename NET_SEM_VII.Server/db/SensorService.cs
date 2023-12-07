using MongoDB.Bson;
using MongoDB.Driver;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NET_SEM_VII.Server.db
{
    public class SensorsService
    {
        private readonly IMongoDatabase _db;
        public IMongoCollection<Entity> Entities { get; }
        private string collectionName = "SensorsData";
        public SensorsService()
        {
            var client = new MongoClient("mongodb://root:rootpassword@localhost:27017");
            //var client = new MongoClient("mongodb://localhost:27017");
            _db = client.GetDatabase("db");
           // DropCollection();
            CreateCollection();
            Entities = _db.GetCollection<Entity>(collectionName);
        }
        public void DropCollection()
        {
            if (_db.ListCollectionNames().ToList().Contains(collectionName))
            {
                _db.DropCollection(collectionName);
            }
            else
            {
                Console.WriteLine("Collection do not exist!");
            }
        }
        public async void CreateCollection()
        {
            if (!_db.ListCollectionNames().ToList().Contains(collectionName))
            {
                _db.CreateCollection(collectionName);
            }
            else
            {
                Console.WriteLine("Collection already exist!");
            }

        }
        public async Task<List<Entity>> GetAllEntities()
        {
            return await Entities.Find(_ => true).ToListAsync();
        }
        public List<Entity> GetLastHundred(string id)
        {
            var builderSorter = Builders<Entity>.Sort;
            SortDefinition<Entity> sort = builderSorter.Ascending("Date");

            return Entities.Find(e => e.SensorType == id).SortBy(e => e.Date).ToList().Take(100).ToList();
        }  
        public async Task<List<Entity>> GetWithFiltersAndSort(string? type, string? id, DateTime? minDate = null, DateTime? maxDate = null,string sortOrder = "Ascending", string? sortBy = null)
        {
            var builder = Builders<Entity>.Filter;
            SortDefinition<Entity>? a;
            if (sortBy != null)
            {
                var builderSorter = Builders<Entity>.Sort;
                a =  builderSorter.Ascending(sortBy);
            }
            var filter = builder.Eq(x => x.SensorType,type) & builder.Eq(x=> x.SensorId,id) & builder.Lte("Date", maxDate ?? DateTime.MaxValue) & builder.Gte("Date", minDate ?? DateTime.MinValue);
            return await Entities.Find(filter).ToListAsync();
            
        }

        public async Task<List<Entity>> GetAllEntitiesByType(string type)
        {
            return await Entities.Find(e => e.SensorType == type).ToListAsync();
        }
        public async Task<List<Entity>> GetAllEntitiesByID(string id)
        {
            return await Entities.Find(e => e.SensorId == id).ToListAsync();
        }
        public async void SaveEntity(Entity entity)
        {
            await Entities.InsertOneAsync(entity);
        }

        public async void SaveEntities(List<Entity> entities)
        {
            await Entities.InsertManyAsync(entities);
        }
    }
}
