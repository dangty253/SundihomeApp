using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.Views;
using SundihomeApp.Views.Furniture;
using Xamarin.Forms;

namespace SundihomeApp.Helpers
{
    public class NotificationHelper
    {
        public async static Task SendNotification(FirebaseNotificationModel model)
        {
            var client = Helpers.BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + Configuration.ApiConfig.FIREBASE_SERVER_KEY);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string objContent = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(objContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);
        }
        public async static void HandleTapNotification(NotificationModel notification, NotificationService notificationService = null)
        {
            if (notificationService == null)
            {
                notificationService = new NotificationService();
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notificationService.UpdateNotification(notification.Id, true);
                INotificationBadge notificationBadge = DependencyService.Get<INotificationBadge>();

                var currentBadgeCount = notificationBadge.Get();
                int newBadgeCount = currentBadgeCount - 1;
                notificationBadge.Set(newBadgeCount >= 0 ? newBadgeCount : 0);
            }


            if (notification.NotificationType == NotificationType.ViewPost && notification.PostId.HasValue) //ViewPost
            {
                await Shell.Current.Navigation.PushAsync(new PostDetailPage(notification.PostId.Value));
            }
            else if (notification.NotificationType == NotificationType.ViewAppointment && notification.AppointmentId != Guid.Empty) //ViewAppointment
            {
                await Shell.Current.Navigation.PushAsync(new AppointmentPage(notification.AppointmentId));
            }
            else if (notification.NotificationType == NotificationType.ViewPostItem && string.IsNullOrWhiteSpace(notification.PostItemId) == false)
            {
                await Shell.Current.Navigation.PushAsync(new PostItemDetailPage(notification.PostItemId));
            }
            else if (notification.NotificationType == NotificationType.VIewFurniturePostItem && string.IsNullOrWhiteSpace(notification.PostItemId) == false)
            {
                await Shell.Current.Navigation.PushAsync(new FurniturePostItemDetailPage(notification.PostItemId));
            }
            else if (notification.NotificationType == NotificationType.ViewLiquidationPostItem && string.IsNullOrWhiteSpace(notification.PostItemId) == false)
            {
                await Shell.Current.Navigation.PushAsync(new Views.LiquidationViews.PostItemDetailPage(notification.PostItemId));
            }
            else if (notification.NotificationType == NotificationType.ViewB2BPostItem && string.IsNullOrWhiteSpace(notification.PostItemId) == false)
            {
                await Shell.Current.Navigation.PushAsync(new Views.CompanyViews.B2BDetailPage(notification.PostItemId));
            }
            else if (notification.NotificationType == NotificationType.ViewInternalPostItem && string.IsNullOrWhiteSpace(notification.PostItemId) == false)
            {
                await Shell.Current.Navigation.PushAsync(new Views.CompanyViews.InternalDetailPage(notification.PostItemId));
            }
            else if (notification.NotificationType == NotificationType.ViewMessage && notification.ChatUserId != null)
            {
                var navi = Shell.Current.Navigation;
                var naviStack = navi.NavigationStack;
                Page lastPage = naviStack[naviStack.Count - 1];
                if (lastPage != null && lastPage.GetType() == typeof(ChatPage))
                {
                    await navi.PopAsync();
                }
                await navi.PushAsync(new ChatPage(notification.ChatUserId));
            }
            else if (notification.NotificationType == NotificationType.RegisterEmployeeSuccess)
            {
                // lay lai thong tin dang ky
                var response = await ApiHelper.Get<User>(ApiRouter.USER_GET_USER_BY_ID + "/" + UserLogged.Id);
                if (response.IsSuccess)
                {
                    var userData = response.Content as User;
                    UserLogged.Type = userData.Type.HasValue ? userData.Type.Value : 0;
                    UserLogged.RoleId = userData.RoleId.HasValue ? userData.RoleId.Value : -1;
                    UserLogged.CompanyId = userData.CompanyId.HasValue ? userData.CompanyId.Value.ToString() : null;

                    var appShell = (AppShell)Shell.Current;
                    appShell.AddMenu_QuanLyCongTy();
                    appShell.AddMenu_QuanLyMoiGioi();
                    await Shell.Current.GoToAsync("//" + AppShell.QUANLYCONGTY);
                }
            }
            else if (notification.NotificationType == NotificationType.UpdateVersion)
            {
                var result = await Shell.Current.DisplayAlert("Cập nhật", "Cập nhật ứng dụng để trải nghiệm tính năng mới", "Cập nhật", Language.dong);
                if (result)
                {
                    DependencyService.Get<IOpenApp>().OpenAppStore();
                }
            }
        }
        public async static Task SaveToken()
        {
            var newToken = Plugin.FirebasePushNotification.CrossFirebasePushNotification.Current.Token;
            if (UserLogged.IsLogged && UserLogged.FirebaseRegToken != newToken)
            {
                await ApiHelper.Put("api/user/firebasetoken/" + newToken, null, true);
                UserLogged.FirebaseRegToken = newToken;
            }
        }
    }
}
