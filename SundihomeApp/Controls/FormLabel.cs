using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
namespace SundihomeApp.Controls
{
    public class FormLabel : Label
    {
        public FormLabel()
        {
            FontSize = 15;
            FontAttributes = FontAttributes.Bold;
            TextColor = Color.FromHex("#444444");
            Margin = new Thickness(4, 5, 0, 0);
        }
    }
}
