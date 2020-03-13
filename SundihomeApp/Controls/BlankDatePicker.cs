using System;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class BlankDatePicker: DatePicker
    {
        public BlankDatePicker()
        {
            TextColor = Color.FromHex("444");
            HeightRequest = 40;
            Margin = new Thickness(15, 5);
            FontSize = 15;
        }
    }
}
