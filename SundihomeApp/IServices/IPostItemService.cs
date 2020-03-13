using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.Mongodb;

namespace SundihomeApp.IServices
{
    public interface IPostItemService
    {
        List<PostItem> GetPostItems(int page);
        Task<PostItem> GetById(string Id);
        void AddPostItem(PostItem postItem);
        Task RemovePostItem(string Id);
        List<PostItemComment> GetComment(string PostItemId, int Page);
        Task InsertComment(PostItemComment comment);
        Task<string[]> GetReceiveNotificationUser(string PostId);
        Task<bool> Follow(string PostId, string UserId);
        DateTime? GatLastPostTime(string UserId);
    }
}
