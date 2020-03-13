using System;
using System.Collections.Generic;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class DangKyMoiGioiPage : ContentPage
    {
        public DangKyMoiGioiPage()
        {
            InitializeComponent();
            var dangKyMoiGioiContentView = new DangKyMoiGioiContentView(LookUpModal, Guid.Parse(UserLogged.Id));
            dangKyMoiGioiContentView.OnSaved += async (object s, EventArgs e2) =>
            {
                await Shell.Current.GoToAsync("//" + AppShell.QUANLYMOIGIOI);
            };
            dangKyMoiGioiContentView.OnCancel += async (object ssender, EventArgs e2) => await Navigation.PopAsync();
            MainContentView.Content = dangKyMoiGioiContentView;
        }
    }
}
