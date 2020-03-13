using System;
namespace SundihomeApp.IServices
{
    public interface INotificationBadge
    {
        int Get();
        void Set(int count);
    }
}
