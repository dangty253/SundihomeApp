using System;
using System.Collections.Generic;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
            Init();
        }
        public async void Init()
        {
            if (UserLogged.IsLogged)
            {
                imgAvatar.Source = UserLogged.AvatarUrl;
                lblUserName.Text = UserLogged.FullName;
                var tapped = new TapGestureRecognizer()
                {
                    NumberOfTapsRequired = 1,
                };
                tapped.Tapped += GoToUserProfile_Clicked;
                grAccountName.GestureRecognizers.Add(tapped);
            }
            else
            {
                imgAvatar.Source = "https://ui-avatars.com/api/?background=0D8ABC&rounded=true&color=fff&bold=true&size=128&name=U";
                lblUserName.Text = Language.dang_nhap_dang_ky;
                lblThongTin.Text = Language.click_de_dang_nhap_dang_ky;
                var tapped = new TapGestureRecognizer()
                {
                    NumberOfTapsRequired = 1,
                };
                tapped.Tapped += GoToLoginPage_Clicked;
                grAccountName.GestureRecognizers.Add(tapped);
            }
        }
        public void GoToLoginPage_Clicked(object sender, EventArgs e)
        {
            ((AppShell)Shell.Current).SetLoginPageActive();
        }
        public void GoToUserProfile_Clicked(object sender, EventArgs e)
        {
            ((AppShell)Shell.Current).SetAccountPageActive();
        }
        public async void GoToMyProduct_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nha_dang_ky_de_thuc_hien_chuc_nang_nay, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Shell.Current.Navigation.PushAsync(new MyLiquidationListPage());
        }
        public async void GoToProductToDay_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nha_dang_ky_de_thuc_hien_chuc_nang_nay, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Shell.Current.Navigation.PushAsync(new MyToDayListPage());
        }
        public async void GoToDonDatHang_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nha_dang_ky_de_thuc_hien_chuc_nang_nay, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await DisplayAlert("", Language.don_dat_hang, "ok");
        }
        public async void GoToDonMuaHang_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nha_dang_ky_de_thuc_hien_chuc_nang_nay, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await DisplayAlert("", Language.don_mua_hang, "ok");
        }
    }
}
