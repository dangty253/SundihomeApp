using System;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class BlankEditor: Editor
    {
        public BlankEditor()
        {
            PlaceholderColor = Color.Gray;
            Margin = new Thickness(15, 5);
            FontSize = 15;
        }
    }
}
