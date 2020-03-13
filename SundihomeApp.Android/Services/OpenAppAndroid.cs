using System;
using Android.App;
using Android.Content;
using Plugin.CurrentActivity;
using SundihomeApp.Droid.Services;
using SundihomeApp.IServices;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(OpenAppAndroid))]
namespace SundihomeApp.Droid.Services
{
    [Activity(Label = "OpenAppAndroid")]
    public class OpenAppAndroid : Activity, IOpenApp
    {
        Context CurrentContext => CrossCurrentActivity.Current.Activity;
        public OpenAppAndroid() { }

        public void OpenFacebookApp()
        {
            try
            {
                Intent intent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage("com.facebook.katana");
                // If not NULL run the app, if not, take the user to the app store
                if (intent != null)
                {
                    //intent.AddFlags(ActivityFlags.NewTask);
                    //CurrentContext.StartActivity(intent);
                    var uri = Android.Net.Uri.Parse("fb://group/875698939556341");
                    var intent2 = new Intent(Intent.ActionView, uri);
                    CurrentContext.StartActivity(intent2);
                }
                else
                {
                    intent = new Intent(Intent.ActionView);
                    intent.AddFlags(ActivityFlags.NewTask);
                    intent.SetData(Android.Net.Uri.Parse("https://www.facebook.com/groups/875698939556341/")); //intent.SetData(Android.Net.Uri.Parse("market://details?id=com.facebook.katana"));
                    CurrentContext.StartActivity(intent);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void OpenViberApp()
        {
            try
            {
                Intent intent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage("com.viber.voip");
                if (intent != null)
                {
                    var uri = Android.Net.Uri.Parse("viber://contact?number=84918339689");
                    var intent2 = new Intent(Intent.ActionView, uri);
                    CurrentContext.StartActivity(intent2);
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Vui lòng cài đặt ứng dụng để sử dụng tiện ích này.");
                    try
                    {
                        CurrentContext.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.viber.voip")));
                    }
                    catch
                    {
                        CurrentContext.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.viber.voip")));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void OpenAppStore()
        {
            try
            {
                CurrentContext.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.bsdsolutions.sundihomeapp")));
            }
            catch
            {
                CurrentContext.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.bsdsolutions.sundihomeapp")));
            }
        }
    }
}
