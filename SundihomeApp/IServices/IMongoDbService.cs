using System;
using MongoDB.Driver;

namespace SundihomeApp.IServices
{
    public interface IMongoDbService
    {
        IMongoCollection<T> GetCollection<T>(string CollectionName) where T : class;
    }
}
