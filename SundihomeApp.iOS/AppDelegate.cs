using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.CloudMessaging;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using PanCardView.iOS;
using Plugin.FacebookClient;
using Plugin.FirebasePushNotification;
using SundihomeApp.Helpers;
using SundihomeApp.iOS.Services;
using SundihomeApp.IServices;
using SundihomeApp.Views;
using SundihomeApp.Views.Furniture;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
namespace SundihomeApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            ImageCircleRenderer.Init();
            Stormlion.PhotoBrowser.iOS.Platform.Init();
            DependencyService.Register<IMediaPickerService, MediaPickerService>();
            DependencyService.Register<INotificationBadge, NotificationBadgeImplement>();
            DependencyService.Register<IToastMessage, ToastMessage>();
            DependencyService.Register<OpenAppiOS>();
            DependencyService.Register<IAppVersionAndBuild>();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            // cards view
            CardsViewRenderer.Preserve();

            // fb share
            Plugin.Share.ShareImplementation.ExcludedUIActivityTypes.Clear();

            //gg login
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();

            //fb login
            FacebookClientManager.Initialize(app, options);
            LoadApplication(new App());

            // firebase notification.
            FirebasePushNotificationManager.Initialize(options, true);
            FirebasePushNotificationManager.CurrentNotificationPresentationOption = UNNotificationPresentationOptions.Alert;
            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Console.WriteLine("test message");
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        //gg login
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            switch (url.Scheme)
            {
                case "fb764532897311608":
                    return FacebookClientManager.OpenUrl(app, url, options);

                case "com.googleusercontent.apps.607579750145-7jckghtgn6mhqva1o7762ouceng4mh1k":
                    // Convert NSUrl to Uri
                    var uri = new Uri(url.AbsoluteString);

                    // Load redirectUrl page
                    AuthenticationState.Authenticator.OnPageLoading(uri);

                    return true;

                case "sundihome":
                    string fullUrl = url.ToString();
                    if (fullUrl.StartsWith("sundihome://postitem", StringComparison.OrdinalIgnoreCase))
                    {
                        string postItemId = fullUrl.Replace("sundihome://postitem/", null);
                        App.GoToPostItemPage(postItemId);
                    }
                    else if (fullUrl.StartsWith("sundihome://post", StringComparison.OrdinalIgnoreCase))
                    {
                        var postId = fullUrl.Replace("sundihome://post/", null);
                        App.GoToPostDetailPage(Guid.Parse(postId));
                    }
                    else if (fullUrl.StartsWith("sundihome://product", StringComparison.OrdinalIgnoreCase))
                    {
                        var productId = fullUrl.Replace("sundihome://product/", null);
                        App.GoToFurnitureProductDetailPage(Guid.Parse(productId));
                    }
                    else if (fullUrl.StartsWith("sundihome://furniture/postitem", StringComparison.OrdinalIgnoreCase))
                    {
                        var furniturePostItemId = fullUrl.Replace("sundihome://furniture/postitem/", null);
                        App.GoToFurniturePostItemDetailPage(furniturePostItemId);
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
                    return true;
            }
            return base.OpenUrl(app, url, options);
        }

        //fb login
        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            FacebookClientManager.OnActivated();
        }


        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return FacebookClientManager.OpenUrl(application, url, sourceApplication, annotation);
        }

        public async override void WillEnterForeground(UIApplication application)
        {// tu backgroud mo len.
            IHubConnectionService hubConnectionService = DependencyService.Get<IHubConnectionService>();
            await hubConnectionService.ReStart();
            Console.WriteLine("App will enter foreground");
        }
        public override void OnResignActivation(UIApplication application)
        {
            Console.WriteLine("OnResignActivation called, App moving to inactive state.");
        }
        public async override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("App entering background state.");
            IHubConnectionService hubConnectionService = DependencyService.Get<IHubConnectionService>();
            await hubConnectionService.Stop();
        }
        // not guaranteed that this will run
        public async override void WillTerminate(UIApplication application)
        {
            IHubConnectionService hubConnectionService = DependencyService.Get<IHubConnectionService>();
            await hubConnectionService.Stop();
        }
    }
}
