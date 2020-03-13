using System;
using MongoDB.Driver;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.IServices;
using Xamarin.Forms;

namespace SundihomeApp.Services
{
    public class MessageService
    {
        private MongoClient _client;
        public IMongoCollection<MessageItem> _messages { get; private set; }
        private IMongoDbService _mongoDbService;
        public MessageService()
        {
            _mongoDbService = DependencyService.Get<IMongoDbService>();
            _messages = _mongoDbService.GetCollection<MessageItem>("Messages");
        }
    }
}
