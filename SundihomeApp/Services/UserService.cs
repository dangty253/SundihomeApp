using System;
using MongoDB.Driver;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.IServices;
using Xamarin.Forms;

namespace SundihomeApp.Services
{
    public class UserService : IUserService
    {
        private MongoClient _client;
        public IMongoCollection<PostItemUser> _users { get; private set; }
        private IMongoDbService _mongoDbService;
        public UserService()
        {
            _mongoDbService = DependencyService.Get<IMongoDbService>();
            //_client = new MongoClient(Configuration.ApiConfig.MONGODB_CONNECTIONSTRING);
            //var database = _client.GetDatabase(Configuration.ApiConfig.MONGODB_DBNAME);
            _users = _mongoDbService.GetCollection<PostItemUser>("Users");
        }

        public PostItemUser Find(string Id)
        {
            return _users.Find(x => x.UserId == Id).SingleOrDefault();
        }

        public void Update(PostItemUser user)
        {
            _users.ReplaceOne(x => x.Id == user.Id, user);
        }
    }
}
