using System;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public class DangKyCongTyButtonPage : ContentPage
    {
        public DangKyCongTyButtonPage()
        {
            this.Title = Language.dang_ky_cong_ty;
            this.BackgroundColor = Color.White;
            Shell.SetTabBarIsVisible(this, false);
            var button = new Button()
            {
                Text = Language.dang_ky_cong_ty,
                Padding = new Thickness(10, 5),
                BackgroundColor = (Color)App.Current.Resources["MainDarkColor"],
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            button.Clicked += async (sender, e) =>
            {
                if (!UserLogged.IsLogged)
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_dang_ky_de_tao_cong_ty, Language.dong);
                    ((AppShell)Shell.Current).SetLoginPageActive();
                    return;
                }

                await Shell.Current.GoToAsync("//batdongsan", false);
                await Shell.Current.Navigation.PushAsync(new AddCompanyPage());
            };

            Label lbl = new Label()
            {
                Padding = 20,
                Text = Language.gioi_thieu_khi_dang_ky_cong_ty,
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
    }
}
