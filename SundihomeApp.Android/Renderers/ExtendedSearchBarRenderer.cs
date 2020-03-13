using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using SundihomeApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SearchBar), typeof(ExtendedSearchBarRenderer))]
namespace SundihomeApp.Droid.Renderers
{
    public class ExtendedSearchBarRenderer : SearchBarRenderer
    {
        public ExtendedSearchBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                plate.SetBackgroundColor(Android.Graphics.Color.Transparent);

                SearchView searchView = (base.Control as SearchView);
                var searchIconId = searchView.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
                var searchIcon = searchView.FindViewById(searchIconId);
                (searchIcon as ImageView).SetColorFilter(Android.Graphics.Color.LightGray, PorterDuff.Mode.SrcIn);

                int editTextId = Resources.GetIdentifier("android:id/search_src_text", null, null);
                EditText editText = (Control.FindViewById(editTextId) as EditText);
            }

        }
    }
}
