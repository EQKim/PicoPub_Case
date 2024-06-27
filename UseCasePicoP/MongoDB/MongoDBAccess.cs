using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using UseCasePicoP.ClassLibrary;

namespace UseCasePicoP.MongoDB
{
    public class MongoDBAccess
    {
        private readonly IMongoDatabase _database;

        public MongoDBAccess(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("MongoDB")["ConnectionString"]);
            _database = client.GetDatabase(configuration.GetSection("MongoDB")["DatabaseName"]);
        }

        public IMongoCollection<Item> Items => _database.GetCollection<Item>("Items");
    }
}