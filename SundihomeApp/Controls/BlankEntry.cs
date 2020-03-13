using System;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class BlankEntry: Entry
    {
        public BlankEntry()
        {
            PlaceholderColor = Color.Gray;
            HeightRequest = 40;
            Margin = new Thickness(15, 5);
            FontSize = 15;
        }
    }
}
