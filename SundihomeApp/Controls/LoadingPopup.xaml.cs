using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class LoadingPopup : ContentView
    {
        public LoadingPopup()
        {
            InitializeComponent();
            this.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
        }
    }
}
