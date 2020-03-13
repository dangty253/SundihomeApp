using System;

using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class SearchBarFrame : Frame
    {
        public SearchBarFrame()
        {
            HeightRequest = 35;
            CornerRadius = 8;
            BorderColor = Color.FromHex("#aaaaaa");
            Padding = 0;
            Margin = new Thickness(5, 0);
            HasShadow = false;
        }
    }
}

