using System;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class SearchBar : Xamarin.Forms.SearchBar
    {
        public SearchBar()
        {
            Placeholder = Language.tim_kiem;
            FontSize = 14;
            TextColor = Color.FromHex("#444444");
            if (Device.RuntimePlatform== Device.iOS)
            {
                BackgroundColor = Color.White;
            }
        }
    }
}

