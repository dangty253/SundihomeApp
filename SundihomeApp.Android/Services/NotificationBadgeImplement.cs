using System;
using SundihomeApp.IServices;
using SundihomeApp.Services;

namespace SundihomeApp.Droid.Services
{
    public class NotificationBadgeImplement : INotificationBadge
    {
        public NotificationBadgeImplement()
        {
        }

        public int Get()
        {
            return 5;
        }

        public void Set(int count)
        {
            
        }
    }
}
