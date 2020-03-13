
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SundihomeApp.Droid.Services;
using SundihomeApp.IServices;
using Xamarin.Forms;

namespace SundihomeApp.Droid
{
    [Activity(Label = "Sundihome", Icon = "@drawable/icon_80", Theme = "@style/MyTheme.Splash", MainLauncher = false, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
        }
    }
}
