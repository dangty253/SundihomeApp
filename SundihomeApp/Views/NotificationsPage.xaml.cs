using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Firebase.Database;
using Firebase.Database.Query;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class NotificationsPage : ContentPage
    {
        private NotificationPageViewModel viewModel;

        public NotificationsPage()
        {
            InitializeComponent();
            if (!UserLogged.IsLogged)
            {
                ToolbarItems.Clear();
                return;
            }
            this.BindingContext = viewModel = new NotificationPageViewModel();
            Init();
        }

        public async void Init()
        {
            ListViewNotification.ItemTapped += ListViewNotification_ItemTapped;
            ListViewNotification.ItemAppearing += ListViewNotification_ItemAppearing;
            viewModel.LoadData();
        }
        private void ListViewNotification_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var noti = e.Item as NotificationModel;
            if (noti.Id == viewModel.Data.LastOrDefault().Id)
            {
                viewModel.Page++;
                viewModel.LoadData();
            }
        }

        private async void ListViewNotification_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var notification = e.Item as NotificationModel;
                NotificationHelper.HandleTapNotification(notification, viewModel.notificationService);
            }
            catch
            {
                await DisplayAlert("", "Thông báo này đã bị xoá hoặc bạn không thể xem thông báo này.", Language.dong);
            }
        }

        private void DeleteNotification_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged) return;
            viewModel.notificationService.DeleteNotification(Guid.Parse(UserLogged.Id));
            viewModel.notificationBadge.Set(0);
            viewModel.Data.Clear();
        }
    }
}
