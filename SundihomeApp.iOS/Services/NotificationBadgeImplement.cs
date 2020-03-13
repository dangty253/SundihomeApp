using System;
using SundihomeApp.IServices;
using SundihomeApp.Services;
using UIKit;

namespace SundihomeApp.iOS.Services
{
    public class NotificationBadgeImplement : INotificationBadge
    {
        public int Get()
        {
            return (int)UIApplication.SharedApplication.ApplicationIconBadgeNumber;
        }

        public void Set(int count)
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = count;
        }
    }
}
