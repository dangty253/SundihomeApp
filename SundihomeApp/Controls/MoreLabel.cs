using System;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class MoreLabel:Label
    {
        public event EventHandler OnTapped;

        public MoreLabel()
        {
            //Color.FromHex("#4c4c4c")
            //Color.FromHex("#6b6a6a")
            Margin = new Thickness(5);
            var formattedString = new FormattedString();
            formattedString.Spans.Add(new Span
            {
                Text = Language.see_all,
                TextColor = (Color)App.Current.Resources["MainDarkColor"],
                FontSize = 14,
            }) ;
            formattedString.Spans.Add(new Span { Text = " " });
            formattedString.Spans.Add(new Span {
                Text = "\uf054",
                TextColor = (Color)App.Current.Resources["MainDarkColor"],
                FontSize = 12,
                FontFamily= FontAwesomeHelper.GetFont("FontAwesomeSolid")
            });
            FormattedText = formattedString;


            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Tap_Tapped;
            this.GestureRecognizers.Add(tap);
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            OnTapped?.Invoke(this, EventArgs.Empty);
        }
    }
}
