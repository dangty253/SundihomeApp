using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Services;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class NotificationPageViewModel : BaseViewModel
    {
        public int Page = 1;
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get => new Command(Refresh);
        }

        public ObservableCollection<NotificationModel> Data { get; set; }
        public NotificationService notificationService;
        public INotificationBadge notificationBadge;
        public NotificationPageViewModel()
        {
            notificationService = new NotificationService();
            notificationBadge = DependencyService.Get<INotificationBadge>();
            Data = new ObservableCollection<NotificationModel>();
        }

        public void LoadData()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var data = notificationService.GetNotifications(Page);
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        this.Data.Add(item);
                    }
                }
            });
        }
        public void Refresh()
        {
            Page = 1;
            Data.Clear();
            LoadData();
            IsRefreshing = false;
        }
    }
}
