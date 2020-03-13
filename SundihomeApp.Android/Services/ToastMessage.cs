using System;
using Android.App;
using Android.Widget;
using SundihomeApp.IServices;

namespace SundihomeApp.Droid.Services
{
    public class ToastMessage : IToastMessage
    {
        public Toast test;
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}
