using System;
using System.Collections.Generic;
using SundihomeApp.IServices;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class SupportPage : ContentPage
    {
        public SupportPage()
        {
            InitializeComponent();
        }

        private void Facebook_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IOpenApp>().OpenFacebookApp();
        }
        private async void Zalo_Clicked(object sender, EventArgs e)
        {
            //await DisplayAlert("", "", Language.dong);
        }
        private void Viber_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IOpenApp>().OpenViberApp();
        }
    }
}
