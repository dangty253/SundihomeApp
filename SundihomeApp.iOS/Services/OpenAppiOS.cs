using System;
using System.Diagnostics;
using Foundation;
using SundihomeApp.iOS.Services;
using SundihomeApp.IServices;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(OpenAppiOS))]
namespace SundihomeApp.iOS.Services
{
    public class OpenAppiOS : IOpenApp
    {
        public OpenAppiOS()
        {
        }

        public void OpenAppStore()
        {
            NSUrl request = new NSUrl("itms-apps://itunes.apple.com/app/id1486501873");
            if (UIApplication.SharedApplication.CanOpenUrl(request))
            {
                bool isOpened = UIApplication.SharedApplication.OpenUrl(request);
                if (isOpened == false)
                    UIApplication.SharedApplication.OpenUrl(request);
            }
        }

        public void OpenFacebookApp()
        {
            try
            {
                NSUrl request = new NSUrl("fb://group?id=875698939556341");
                if (!UIApplication.SharedApplication.CanOpenUrl(request))
                {
                    request = new NSUrl("https://www.facebook.com/groups/sundihome.support/");
                }
                bool isOpened = UIApplication.SharedApplication.OpenUrl(request);
                if (isOpened == false)
                    UIApplication.SharedApplication.OpenUrl(request);
            }
            catch
            {

            }
        }

        public void OpenViberApp()
        {
            try
            {
                NSUrl request = new NSUrl("viber://add?number=84918339689");
                var appStoreLink = new NSUrl("https://apps.apple.com/us/app/viber-messenger-chats-calls/id382617920");
                if (UIApplication.SharedApplication.CanOpenUrl(request))
                {
                    bool isOpened = UIApplication.SharedApplication.OpenUrl(request);
                    if (isOpened == false)
                        UIApplication.SharedApplication.OpenUrl(request);
                }
                else if (UIApplication.SharedApplication.CanOpenUrl(appStoreLink))
                {
                    bool isOpened = UIApplication.SharedApplication.OpenUrl(appStoreLink);
                    if (isOpened == false)
                        UIApplication.SharedApplication.OpenUrl(appStoreLink);
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Vui lòng cài đặt ứng dụng Viber để sử dụng tiện ích này.");
                }
            }
            catch
            {

            }
        }
    }
}
