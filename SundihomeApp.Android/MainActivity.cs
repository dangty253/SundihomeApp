using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using Xamarin.Forms;
using SundihomeApp.Droid.Services;
using Android.Content;
using Plugin.CurrentActivity;
using Plugin.FacebookClient;
using Java.Security;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.FirebasePushNotification;
using Android.Gms.Common;
using SundihomeApp.IServices;
using System.Net;
using PanCardView.Droid;

namespace SundihomeApp.Droid
{
    [Activity(Theme = "@style/MainTheme", Icon = "@drawable/icon_80", Label = "Sundihome", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
                       AutoVerify = true,
                       Categories = new[]
                       {
                            Intent.CategoryDefault,
                            Intent.CategoryBrowsable
                       },
                       DataScheme = "sundihome")]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

            base.OnCreate(savedInstanceState);

            // I strongly recommend wrapping this in the compiler directive, because you should have a proper
            // certificate in a production environment.

            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            DependencyService.Register<IMediaPickerService, MediaPickerService>();
            DependencyService.Register<INotificationBadge, NotificationBadgeImplement>();
            DependencyService.Register<IToastMessage, ToastMessage>();
            DependencyService.Register<OpenAppAndroid>();
            DependencyService.Register<IAppVersionAndBuild>();

            // cards view
            CardsViewRenderer.Preserve();

            //gg login
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);

            //fb login
            FacebookClientManager.Initialize(this);
            LoadApplication(new App());

            // firebase notification
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            FacebookClientManager.OnActivityResult(requestCode, resultCode, intent);
            MultiMediaPickerService.SharedInstance.OnActivityResult(requestCode, resultCode, intent);
        }


        // bi che mat 1 phan
        protected async override void OnPause()
        {
            base.OnPause();
            Console.WriteLine("OnPause , App bi che");
            IHubConnectionService hubConnectionService = DependencyService.Get<IHubConnectionService>();
            await hubConnectionService.Stop();
        }

        protected async override void OnResume()
        {
            Console.WriteLine("OnResume restart hub");
            base.OnResume();
            IHubConnectionService hubConnectionService = DependencyService.Get<IHubConnectionService>();
            await hubConnectionService.ReStart();
        }

        public static void PrintHashKey(Context pContext)
        {
            try
            {
                PackageInfo info = Android.App.Application.Context.PackageManager.GetPackageInfo(Android.App.Application.Context.PackageName, PackageInfoFlags.Signatures);
                foreach (var signature in info.Signatures)
                {
                    MessageDigest md = MessageDigest.GetInstance("SHA");
                    md.Update(signature.ToByteArray());

                    System.Diagnostics.Debug.WriteLine(Convert.ToBase64String(md.Digest()));
                }
            }
            catch (NoSuchAlgorithmException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}