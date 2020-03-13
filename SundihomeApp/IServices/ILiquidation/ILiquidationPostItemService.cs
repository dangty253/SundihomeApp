using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.Mongodb.Liquidation;

namespace SundihomeApp.IServices.ILiquidation
{
    public interface ILiquidationPostItemService
    {
        List<LiquidationPostItem> GetPostItems(int page);
        Task<LiquidationPostItem> GetById(string Id);
        void AddPostItem(LiquidationPostItem postItem);
        Task RemovePostItem(string Id);
        List<LiquidationPostItemComment> GetComment(string PostItemId, int Page);
        Task InsertComment(LiquidationPostItemComment comment);
        Task<string[]> GetReceiveNotificationUser(string PostId);
        Task<bool> Follow(string PostId, string UserId);
    }
}
