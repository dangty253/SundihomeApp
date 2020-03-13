using System;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public class DangKyMoiGioiButtonPage : ContentPage
    {
        public DangKyMoiGioiButtonPage()
        {
            this.Title = Language.dang_ky_moi_gioi;
            this.BackgroundColor = Color.White;
            Shell.SetTabBarIsVisible(this, false);
            var button = new Button()
            {
                Text = Language.dang_ky_moi_gioi,
                Padding = new Thickness(10, 5),
                BackgroundColor = (Color)App.Current.Resources["MainDarkColor"],
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            button.Clicked += DangKyMoiGioi_Clicked;

            Label lbl = new Label()
            {
                Padding = 20,
                Text = Language.gioi_thieu_khi_dang_ky_moi_gioi,
                FontSize = 15,
                TextColor = Color.FromHex("#444444")
            };

            StackLayout st = new StackLayout();
            st.VerticalOptions = LayoutOptions.Center;
            st.HorizontalOptions = LayoutOptions.Center;
            st.Children.Add(lbl);
            st.Children.Add(button);
            this.Content = st;
        }


        private async void DangKyMoiGioi_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_de_dang_ky_moi_gioi, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            if (UserLogged.Type == 1)
            {
                await DisplayAlert("", Language.ban_dang_la_moi_gioi, Language.dong);
            }
            await Shell.Current.GoToAsync("//batdongsan", false);
            await Shell.Current.Navigation.PushAsync(new DangKyMoiGioiPage(), false);
        }
    }
}
