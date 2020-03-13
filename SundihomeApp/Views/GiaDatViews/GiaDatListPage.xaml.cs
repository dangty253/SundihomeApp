using System;
using System.Collections.Generic;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Views.GiaDatViews
{
    public partial class GiaDatListPage : ContentPage
    {
        public GiaDatListPage()
        {
            InitializeComponent();
        }

        private async void HoChiMinh_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatHCMPage());
        }
        private async void HaNoi_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatHaNoiPage());
        }
        private async void NamDinh_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatNamDinhPage());
        }
        private async void BinhDuong_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatBinhDuongPage());
        }
        private async void DongNai_Clicked(object sender,EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatDongNaiPage());
        }
        private async void BacNinh_Clicked(object sender,EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatBacNinhPage());
        }
    }
}
