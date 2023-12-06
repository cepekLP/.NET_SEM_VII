using MongoDB.Driver;
using System.ComponentModel;

namespace NET_SEM_VII.Server.db
{
    public class Database
    {
        private readonly IMongoDatabase _db;
        public IMongoCollection<Entity> Entities { get; }
        public Database()
        {
            var client = new MongoClient("mongodb://root:rootpassword@localhost:27017");
            //var client = new MongoClient("mongodb://localhost:27017");
            _db = client.GetDatabase("db");

            _db.CreateCollectionAsync("Entities");
            Entities = _db.GetCollection<Entity>("Entities");
        }

        public async void SaveEntity(Entity entity)
        {
            await Entities.InsertOneAsync(entity);
        }

        public async Task<List<Entity>> GetAllEntities()
        {
            return await Entities.Find(_ => true).ToListAsync();
        }

        public async void Test()
        {
            var ent = new Entity();
            ent.Value = 1.14f;
            ent.Date = DateTime.Now;
            ent.SensorType = "Temp";
            ent.SensorId = "1";
            SaveEntity(ent);

            Thread.Sleep(1000);
            var ListOfEntities = await GetAllEntities();
            Console.WriteLine("Entities:");
            foreach (var en in ListOfEntities)
            {
                Console.WriteLine(en.ToString());
            }
        }
    }
}
