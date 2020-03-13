using System;
using Android.Webkit;
using SundihomeApp.Droid.Services;
using SundihomeApp.IServices;
using SundihomeApp.Services;

[assembly: Xamarin.Forms.Dependency(typeof(IClearCookiesImplementation))]
namespace SundihomeApp.Droid.Services
{
    public class IClearCookiesImplementation : IClearCookies
    {
        public void Clear()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }
    }
}
