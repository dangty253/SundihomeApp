﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Services.Furniture
{
    public class FurniturePostItemService : IFurniturePostItemService
    {
        public IMongoCollection<FurniturePostItem> _postItems { get; private set; }
        public IMongoCollection<FurniturePostItemComment> _postItemComments { get; private set; }

        public FurniturePostItemService()
        {
            var _mongoDbService = DependencyService.Get<IMongoDbService>();
            _postItems = _mongoDbService.GetCollection<FurniturePostItem>("FurniturePostItem");
            _postItemComments = _mongoDbService.GetCollection<FurniturePostItemComment>("FurniturePostItemComment");
        }

        public List<FurniturePostItem> GetPostItems(int page = 1)
        {
            int limit = 10;
            int skip = (page - 1) * limit;
            return _postItems.Find(x => true).SortByDescending(x => x.CreatedDate).Skip(skip).Limit(limit).ToList();
        }
        public async Task<FurniturePostItem> GetById(string Id)
        {
            var postItem = _postItems.Find(x => x.Id == Id).SingleOrDefault();
            IUserService userService = DependencyService.Get<IUserService>();
            postItem.CreatedBy = userService.Find(postItem.CreatedById);
            return postItem;
        }
        public void AddPostItem(FurniturePostItem postItem)
        {
            postItem.UserComments = new List<string>() { postItem.CreatedById };
            this._postItems.InsertOne(postItem);
        }
        public async Task RemovePostItem(string Id)
        {
            var post = await this.GetById(Id);
            var comment = this.GetComment(post.Id);
            if (post.HasImage)
            {
                var response = await ApiHelper.Delete(Configuration.ApiRouter.DELETE_IMAGE + "?bucketName=sundihome/furniture/post&files=" + string.Join(",", post.Images));
            }
            this._postItems.DeleteOne(x => x.Id == Id);
            await this._postItemComments.DeleteManyAsync(x => x.PostItemId == Id);
        }

        public List<FurniturePostItemComment> GetComment(string PostItemId, int Page = 1)
        {
            int Limit = 10;
            int Skip = (Page - 1) * Limit;
            return _postItemComments.Find(x => x.PostItemId == PostItemId).SortByDescending(x => x.CreatedDate).Limit(Limit).Skip(Skip).ToList();
        }


        public async Task InsertComment(FurniturePostItemComment comment)
        {
            var post = await this.GetById(comment.PostItemId);
            // nguoi nay chua co trong danh asch
            if (post.UserComments.Any(x => x == comment.CreatedById) == false)
            {
                post.UserComments.Add(comment.CreatedById);
                _postItems.ReplaceOne(p => p.Id == post.Id, post);
            }
            _postItemComments.InsertOne(comment);
        }

        public async Task<bool> Follow(string PostId, string UserId)
        {
            FurniturePostItem post = _postItems.Find(x => x.Id == PostId).SingleOrDefault();
            if (post.UserFollows != null && post.UserFollows.Any(x => x == UserId)) // dang follow
            {
                post.UserFollows.Remove(UserId);
                _postItems.ReplaceOne(p => p.Id == PostId, post);
                return false;
            }
            else
            {
                if (post.UserFollows == null)
                {
                    post.UserFollows = new List<string>();
                }

                post.UserFollows.Add(UserId);
                _postItems.ReplaceOne(p => p.Id == PostId, post);

                // send notifiaction.
                INotificationService notificationService = DependencyService.Get<INotificationService>();
                Guid ReceiverId = Guid.Parse(post.CreatedById);

                string NotificationImage = (post.Images != null && post.Images.Length > 0) ? AvatarHelper.GetPostAvatar(post.Images.FirstOrDefault()) : null;
                NotificationModel notification = new NotificationModel()
                {
                    UserId = ReceiverId,
                    CurrentBadgeCount = (int)notificationService.CountNotReadNotificationUser(ReceiverId) + 1,
                    Title = UserLogged.FullName + " đã theo dõi bài viết của bạn",
                    NotificationType = NotificationType.VIewFurniturePostItem,
                    PostItemId = PostId,
                    CreatedDate = DateTime.Now,
                    IsRead = false,
                    Thumbnail = NotificationImage
                };
                await notificationService.AddNotification(notification, Language.theo_doi);
                return true;
            }
        }

        public async Task<string[]> GetReceiveNotificationUser(string PostId)
        {
            FurniturePostItem post = await this.GetById(PostId);
            var followList = post.UserFollows ?? new List<string>();
            var commentArray = post.UserComments.ToArray();

            foreach (var item in commentArray)
            {
                if (!followList.Any(x => x == item))
                {
                    followList.Add(item);
                }
            }
            return followList.ToArray();
        }
    }
}
