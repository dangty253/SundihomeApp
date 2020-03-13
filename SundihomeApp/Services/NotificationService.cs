using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Services
{
    public class NotificationService : INotificationService
    {

        private MongoClient _client;
        public IMongoCollection<NotificationModel> _NotificationList { get; private set; }
        public NotificationService()
        {
            _client = new MongoClient(Configuration.ApiConfig.MONGODB_CONNECTIONSTRING);
            var database = _client.GetDatabase(Configuration.ApiConfig.MONGODB_DBNAME);
            _NotificationList = database.GetCollection<NotificationModel>("Notifications");
        }

        public void InitOnStart()
        {
            if (UserLogged.IsLogged)
            {
                INotificationBadge notificationBadge = DependencyService.Get<INotificationBadge>();
                var count = this.CountNotReadNotificationUser(Guid.Parse(UserLogged.Id));
                if (count > 0)
                {

                    notificationBadge.Set((int)count);
                }
                else
                {
                    notificationBadge.Set(0);
                }
            }
        }

        public long CountNotReadNotificationUser(Guid UserId)
        {
            long count = _NotificationList.CountDocuments(x => x.UserId == UserId && x.IsRead == false);
            return count;
        }

        public List<NotificationModel> GetNotifications(int page = 1)
        {
            int limit = 10;
            int skip = (page - 1) * limit;
            return _NotificationList.Find(x => x.UserId == Guid.Parse(UserLogged.Id)).SortByDescending(x => x.CreatedDate).Skip(skip).Limit(limit).ToList();
        }

        public async Task AddNotification(NotificationModel model, string title)
        {
            int badgeCount = (int)this.CountNotReadNotificationUser(model.UserId) + 1;
            model.CurrentBadgeCount = badgeCount;


            _NotificationList.InsertOne(model);

            var response = await ApiHelper.Get<object>($"api/user/usertoken/{model.UserId}");
            if (response.IsSuccess == false || response.Content == null)
            {
                return;
            }

            string ReceiverToken = response.Content.ToString();

            await NotificationHelper.SendNotification(new FirebaseNotificationModel()
            {
                to = ReceiverToken,
                notification = new FirebaseNotification()
                {
                    title = title,
                    body = model.Title,
                    badge = model.CurrentBadgeCount
                },
                data = new Dictionary<string, object>()
                        {
                            {
                                "NotificationData", model
                            },
                        }
            });
        }

        public void UpdateNotification(string Id, bool IsRead)
        {
            try
            {
                var notification = _NotificationList.Find(x => x.Id == Id).SingleOrDefault();
                notification.IsRead = IsRead;
                _NotificationList.ReplaceOne(x => x.Id == Id, notification);
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteNotification(Guid UserId)
        {
            _NotificationList.DeleteMany(x => x.UserId == UserId);
        }
    }
}
