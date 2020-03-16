using System;
using Xamarin.Forms;
using SundihomeApp.Views;
using Plugin.FirebasePushNotification;
using SundihomeApp.Services;
using SundihomeApp.Models;
using Newtonsoft.Json;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Services.Furniture;
using SundihomeApp.Views.Furniture;
using System.Threading.Tasks;
using SundihomeApp.Services.Liquidation;
using SundihomeApp.IServices.ILiquidation;
using System.Globalization;
using System.Threading;
using SundihomeApp.Settings;
using SundihomeApp.Resources;

namespace SundihomeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            App.SetCultureInfo(LanguageSettings.Language);
            MainPage = new AppShell();
            RegisterDependency();
        }

        public static void SetCultureInfo(string code)
        {
            LanguageSettings.Language = code;
            CultureInfo cultureInfo = new CultureInfo(code);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Language.Culture = cultureInfo;
        }

        private void RegisterDependency()
        {
            DependencyService.Register<IPostItemService, PostItemService>();
            DependencyService.Register<IUserService, UserService>();
            DependencyService.Register<INotificationService, NotificationService>();
            DependencyService.Register<IMongoDbService, MongoDbService>();
            DependencyService.Register<IHubConnectionService, HubConnectionService>();
            DependencyService.Register<ICompanyService, CompanyService>();
            DependencyService.Register<IFurniturePostItemService, FurniturePostItemService>();
            DependencyService.Register<ILiquidationCategoryService, LiquidationCategoryService>();
            DependencyService.Register<ILiquidationPostItemService, LiquidationPostItemService>();
            DependencyService.Register<IB2BPostItemService, B2BPostItemService>();
            DependencyService.Register<IInternalPostItemService, InternalPostItemService>();
        }

        protected override void OnStart()
        {
            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
             {
                 await NotificationHelper.SaveToken();
             };
            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                //if (!string.IsNullOrEmpty(p.Identifier))
                //{
                //    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                //    foreach (var data in p.Data)
                //    {
                //        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                //    }
                //}
            };
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
           {
               if (p.Data.ContainsKey("NotificationData"))
               {
                   string NotificationJson = p.Data["NotificationData"].ToString();
                   NotificationModel notification = JsonConvert.DeserializeObject<NotificationModel>(NotificationJson);
                   NotificationHelper.HandleTapNotification(notification);
               }
           };
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                if (p.Data.ContainsKey("NotificationData"))
                {
                    string NotificationJson = p.Data["NotificationData"].ToString();
                    NotificationModel notification = JsonConvert.DeserializeObject<NotificationModel>(NotificationJson);
                    INotificationBadge notificationBadge = DependencyService.Get<INotificationBadge>();
                    notificationBadge.Set(notification.CurrentBadgeCount);
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
            };
        }


        public static void GoToPostItemPage(string postItemId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.BATDONGSAN_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new PostItemDetailPage(postItemId));
        }

        public static void GoToPostDetailPage(Guid PostId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.BATDONGSAN_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new PostDetailPage(PostId));
        }

        public static void GoToFurniturePostItemDetailPage(string postItemId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.FURNITURE_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new FurniturePostItemDetailPage(postItemId));
        }
        public static void GoToFurnitureProductDetailPage(Guid ProductId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.FURNITURE_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new ProductDetailPage(ProductId));
        }
        public static void GoToLiquidationDetailPage(Guid LiquidationId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.LIQUIDATION_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new Views.LiquidationViews.LiquidationDetailPage(LiquidationId));
        }
        public static void GoToLiquidationToDayDetailPage(Guid ToDayId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.LIQUIDATION_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new Views.LiquidationViews.ToDayDetailPage(ToDayId));
        }
        public static void GoToLiquidationPostItemDetailPage(string postItemId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.LIQUIDATION_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new Views.LiquidationViews.PostItemDetailPage(postItemId));
        }

        public static void GoToThongTinMoiGioiPage(Guid moigioiId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.COMPANY_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new Views.MoiGioiViews.ThongTinMoiGioiPage(moigioiId, false));
        }
        public static void GoToB2BDetailPage(string postItemId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.COMPANY_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new Views.CompanyViews.B2BDetailPage(postItemId));
        }
        public static void GoToCompanyDetailPage(Guid companyId)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.COMPANY_INDEX];
            appShell.CurrentItem.Navigation.PushAsync(new Views.CompanyViews.CompanyProfileDetailPage(companyId));
        }
        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            string fullUrl = uri.ToString();
            if (fullUrl.StartsWith("sundihome://postitem", StringComparison.OrdinalIgnoreCase))
            {
                string postItemId = fullUrl.Replace("sundihome://postitem/", null);
                GoToPostItemPage(postItemId);
            }
            else if (fullUrl.Contains("sundihome://post"))
            {
                var id = fullUrl.Replace("sundihome://post/", null);
                GoToPostDetailPage(Guid.Parse(id));
            }
            else if (fullUrl.StartsWith("sundihome://product", StringComparison.OrdinalIgnoreCase))
            {
                var productId = fullUrl.Replace("sundihome://product/", null);
                GoToFurnitureProductDetailPage(Guid.Parse(productId));
            }
            else if (fullUrl.StartsWith("sundihome://furniture/postitem", StringComparison.OrdinalIgnoreCase))
            {
                var furniturePostItemId = fullUrl.Replace("sundihome://furniture/postitem/", null);
                GoToFurniturePostItemDetailPage(furniturePostItemId);
            }
            else if (fullUrl.StartsWith("sundihome://liquidation/postitem", StringComparison.OrdinalIgnoreCase))
            {
                var liquidationId = fullUrl.Replace("sundihome://liquidation/postitem/", null);
                App.GoToLiquidationPostItemDetailPage(liquidationId);
            }
            else if (fullUrl.StartsWith("sundihome://liquidation/today", StringComparison.OrdinalIgnoreCase))
            {
                var liquidationId = fullUrl.Replace("sundihome://liquidation/today/", null);
                App.GoToLiquidationToDayDetailPage(Guid.Parse(liquidationId));
            }
            else if (fullUrl.StartsWith("sundihome://liquidation", StringComparison.OrdinalIgnoreCase))
            {
                var liquidationId = fullUrl.Replace("sundihome://liquidation/", null);
                App.GoToLiquidationDetailPage(Guid.Parse(liquidationId));
            }
            else if (fullUrl.StartsWith("sundihome://moigioi/information", StringComparison.OrdinalIgnoreCase))
            {
                var moigioiId = fullUrl.Replace("sundihome://moigioi/information/", null);
                App.GoToThongTinMoiGioiPage(Guid.Parse(moigioiId));
            }
            else if (fullUrl.StartsWith("sundihome://company/b2bpostitem", StringComparison.OrdinalIgnoreCase))
            {
                var postItemId = fullUrl.Replace("sundihome://company/b2bpostitem/", null);
                App.GoToB2BDetailPage(postItemId);
            }
            else if (fullUrl.StartsWith("sundihome://company", StringComparison.OrdinalIgnoreCase))
            {
                var companyId = fullUrl.Replace("sundihome://company/", null);
                App.GoToCompanyDetailPage(Guid.Parse(companyId));
            }
            base.OnAppLinkRequestReceived(uri);
        }
    }
}
