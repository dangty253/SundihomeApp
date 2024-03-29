﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.Mongodb;

namespace SundihomeApp.IServices
{
    public interface IInternalPostItemService
    {
        List<InternalPostItem> GetPostItems(int page);
        Task<InternalPostItem> GetById(string Id);
        void AddPostItem(InternalPostItem postItem);
        Task RemovePostItem(string Id);
        List<PostItemComment> GetComment(string PostItemId, int Page);
        Task InsertComment(PostItemComment comment);
        Task<string[]> GetReceiveNotificationUser(string PostId);
        Task<bool> Follow(string PostId, string UserId);
        DateTime? GatLastPostTime(string UserId);
    }
}
