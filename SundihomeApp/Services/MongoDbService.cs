using System;
using MongoDB.Driver;
using SundihomeApp.IServices;

namespace SundihomeApp.Services
{
    public class MongoDbService : IMongoDbService
    {
        private MongoClient _client;
        private IMongoDatabase database;
        public MongoDbService()
        {
            _client = new MongoClient(Configuration.ApiConfig.MONGODB_CONNECTIONSTRING);
            database = _client.GetDatabase(Configuration.ApiConfig.MONGODB_DBNAME);
        }
        public IMongoCollection<T> GetCollection<T>(string CollectionName) where T : class
        {
            return database.GetCollection<T>(CollectionName);
        }
    }
}
