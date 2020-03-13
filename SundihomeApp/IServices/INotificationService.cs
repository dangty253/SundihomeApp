using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApp.Models;

namespace SundihomeApp.IServices
{
    public interface INotificationService
    {
       void InitOnStart();
        long CountNotReadNotificationUser(Guid UserId);
        List<NotificationModel> GetNotifications(int page = 1);
        Task AddNotification(NotificationModel model,string title);
        void UpdateNotification(string Id, bool IsRead);
        void DeleteNotification(Guid UserId);
    }
}