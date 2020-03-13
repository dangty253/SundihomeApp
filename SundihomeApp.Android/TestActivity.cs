
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

namespace SundihomeApp.Droid
{
    [Activity(Label = "TestActivity")]
    public class TestActivity : Activity
    {
        Button btnLogin;
        EditText txtUsername;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.TestLayout);
            Title = "Test activity";
            btnLogin = (Button)FindViewById(Resource.Id.btnLogin);
            txtUsername = (EditText)FindViewById(Resource.Id.editText1);
            btnLogin.Click += (o, e) =>
            {
                Toast.MakeText(ApplicationContext, txtUsername.Text, ToastLength.Long);
            };
        }
    }
}
