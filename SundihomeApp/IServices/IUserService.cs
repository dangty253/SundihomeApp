using SundihomeApi.Entities.Mongodb;

namespace SundihomeApp.IServices
{
    public interface IUserService
    {
        PostItemUser Find(string Id);
        void Update(PostItemUser user);
    }
}