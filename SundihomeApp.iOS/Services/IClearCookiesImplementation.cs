using System;
using Foundation;
using SundihomeApp.iOS.Services;
using SundihomeApp.IServices;
using SundihomeApp.Services;

[assembly: Xamarin.Forms.Dependency(typeof(IClearCookiesImplementation))]
namespace SundihomeApp.iOS.Services
{
    public class IClearCookiesImplementation : IClearCookies
    {
        public void Clear()
        {
            NSHttpCookieStorage CookieStorage = NSHttpCookieStorage.SharedStorage;
            foreach (var cookie in CookieStorage.Cookies)
                CookieStorage.DeleteCookie(cookie);
        }
    }
}
