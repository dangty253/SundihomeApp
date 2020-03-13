using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.Mongodb.Furniture;

namespace SundihomeApp.IServices.IFurniture
{
    public interface IFurniturePostItemService
    {
        List<FurniturePostItem> GetPostItems(int page);
        Task<FurniturePostItem> GetById(string Id);
        void AddPostItem(FurniturePostItem postItem);
        Task RemovePostItem(string Id);
        List<FurniturePostItemComment> GetComment(string PostItemId, int Page);
        Task InsertComment(FurniturePostItemComment comment);
        Task<string[]> GetReceiveNotificationUser(string PostId);
        Task<bool> Follow(string PostId, string UserId);
    }
}
