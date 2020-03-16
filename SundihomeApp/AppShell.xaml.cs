using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views;
using SundihomeApp.Views.CompanyViews;
using SundihomeApp.Views.MoiGioiViews;
using Xamarin.Forms;

namespace SundihomeApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public static IDictionary<string, string> Languages;

        public const int BATDONGSAN_INDEX = 0;
        public const int PROJECT_INDEX = 1;
        public const int FURNITURE_INDEX = 2;
        public const int LIQUIDATION_INDEX = 3;
        public const int COMPANY_INDEX = 5;

        public const string QUANLYMOIGIOI = "quanlymoigioi";
        public const string QUANLYCONGTY = "quanlycongty";

        public AppShell()
        {
            InitializeComponent();
            Languages = new Dictionary<string, string>() {
                {"vi", Language.tieng_viet },
                {"en",Language.tieng_anh }
            };
            Init();
        }

        public async void Init()
        {
            if (UserLogged.IsLogged)
            {
                await UserLogged.ReloadProfile();

                AccountShellContent.Content = new AccountPage();
                SetLogoutMenuItem();
            }
            else
            {
                AccountShellContent.Content = new LoginPage();
            }
            AddMenu_QuanLyCongTy();
            AddMenu_QuanLyMoiGioi();
        }

        public async void ToAboutPage_Clicked(object sender, EventArgs e)
        {
            Current.FlyoutIsPresented = false;
            await Current.Navigation.PushAsync(new AboutPage());
        }

        public void SetLoginPageActive()
        {
            LoginPage loginPage = new LoginPage();
            Shell.SetTabBarIsVisible(loginPage, false);
            Shell.Current.Navigation.PushAsync(loginPage);
        }

        public void SetAccountPageActive()
        {
            Shell.Current.GoToAsync("//account");
        }

        public void AddMenu_QuanLyCongTy()
        {
            QuanLyCongTyFlyoutItem.Items.Clear();
            if (UserLogged.IsLogged && !string.IsNullOrEmpty(UserLogged.CompanyId)) // da co cong ty
            {
                ShellContent homePage = new ShellContent();
                homePage.Title = Language.home;
                homePage.Icon = "ic_home.png";
                homePage.ContentTemplate = new DataTemplate(typeof(Views.CompanyViews.QuanLyCongTyViews.HomePage));
                QuanLyCongTyFlyoutItem.Items.Add(homePage);

                int role = UserLogged.RoleId;

                if (role == 0)
                {
                    ShellContent khachhang = new ShellContent();
                    khachhang.Route = "quanlykhachhang";
                    khachhang.Title = Language.khach_hang;
                    khachhang.Icon = "ic_khachhang.png";
                    khachhang.ContentTemplate = new DataTemplate(() => new ContactListPage(true));
                    QuanLyCongTyFlyoutItem.Items.Add(khachhang);
                }

                ShellContent noibo = new ShellContent();
                noibo.Title = Language.noi_bo;
                noibo.Icon = "ic_noibo.png";
                noibo.ContentTemplate = new DataTemplate(typeof(InternalPage));
                QuanLyCongTyFlyoutItem.Items.Add(noibo);

                if (role == 0)
                {
                    ShellContent nhanvien = new ShellContent();
                    nhanvien.Title = Language.nhan_vien;
                    nhanvien.Icon = "ic_nhanvien.png";
                    nhanvien.ContentTemplate = new DataTemplate(typeof(EmployeeListPage));
                    QuanLyCongTyFlyoutItem.Items.Add(nhanvien);
                }

                ShellContent sanpham = new ShellContent();
                sanpham.Route = "sanpham_congty";
                sanpham.Title = Language.san_pham;
                sanpham.Icon = "ic_product.png";
                sanpham.ContentTemplate = new DataTemplate(typeof(Views.CompanyViews.QuanLyCongTyViews.ProductPage));

                QuanLyCongTyFlyoutItem.Items.Add(sanpham);
            }
            else
            {
                ShellContent DangKyCongTyShellContentPage = new ShellContent();
                DangKyCongTyShellContentPage.ContentTemplate = new DataTemplate(typeof(DangKyCongTyButtonPage));
                QuanLyCongTyFlyoutItem.Items.Add(DangKyCongTyShellContentPage);
            }
        }

        public void AddMenu_QuanLyMoiGioi()
        {
            QuanLyMoiGioiFlyoutItem.Items.Clear();
            if (UserLogged.IsLogged && UserLogged.Type == 1)
            {
                ShellContent homePage = new ShellContent();
                homePage.Title = Language.home;
                homePage.Icon = "ic_home.png";
                homePage.ContentTemplate = new DataTemplate(typeof(Views.MoiGioiViews.HomePage));
                QuanLyMoiGioiFlyoutItem.Items.Add(homePage);

                ShellContent congviecPage = new ShellContent();
                congviecPage.Route = "congviec";
                congviecPage.Title = Language.cong_viec;
                congviecPage.Icon = "ic_list.png";
                congviecPage.ContentTemplate = new DataTemplate(typeof(Views.QuanLyMoiGioiViews.TaskListPage));
                QuanLyMoiGioiFlyoutItem.Items.Add(congviecPage);

                if (!string.IsNullOrEmpty(UserLogged.CompanyId))
                {
                    ShellContent noiboPage = new ShellContent();
                    noiboPage.Route = "noibo";
                    noiboPage.Title = Language.noi_bo;
                    noiboPage.Icon = "ic_noibo.png";
                    noiboPage.ContentTemplate = new DataTemplate(typeof(InternalPage));
                    QuanLyMoiGioiFlyoutItem.Items.Add(noiboPage);
                }

                ShellContent giohangPage = new ShellContent();
                giohangPage.Route = "giohang";
                giohangPage.Title = Language.gio_hang;
                giohangPage.Icon = "ic_product.png";
                giohangPage.ContentTemplate = new DataTemplate(typeof(Views.MoiGioiViews.PostListPage));
                QuanLyMoiGioiFlyoutItem.Items.Add(giohangPage);

                ShellContent khachhangPage = new ShellContent();
                khachhangPage.Route = "khachhang";
                khachhangPage.Title = Language.khach_hang;
                khachhangPage.Icon = "ic_khachhang.png";
                khachhangPage.ContentTemplate = new DataTemplate(typeof(ContactListPage));
                QuanLyMoiGioiFlyoutItem.Items.Add(khachhangPage);
            }
            else
            {
                ShellContent DangKyMoiGioiShellContent = new ShellContent();
                DangKyMoiGioiShellContent.ContentTemplate = new DataTemplate(typeof(DangKyMoiGioiButtonPage));
                QuanLyMoiGioiFlyoutItem.Items.Add(DangKyMoiGioiShellContent);
            }
        }

        public void SetLogoutMenuItem()
        {
            var logoutMenuItem = new MenuItem()
            {
                Text = Language.dang_xuat,
                IconImageSource = "ic_logout.png",
            };
            logoutMenuItem.Clicked += Logout_Clicked;
            this.Items.Add(logoutMenuItem);
        }


        public async void ChangeLanguage_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet(Language.chon_ngon_ngu, Language.huy, null, Languages.Values.ToArray());

            if (result == Languages["vi"])
            {
                App.SetCultureInfo("vi");
                Application.Current.MainPage = new AppShell();
            }
            else if (result == Languages["en"])
            {
                App.SetCultureInfo("en");
                Application.Current.MainPage = new AppShell();
            }
        }

        private async void GoiVay_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//congty/congty_goivay");
            Shell.Current.FlyoutIsPresented = false;
        }

        public async void Logout_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert(Language.thong_bao, Language.ban_co_chac_chan_muon_dang_xuat, Language.dong_y, Language.huy);
            if (!result) return;

            string language = LanguageSettings.Language;

            await ApiHelper.Post(ApiRouter.USER_LOGGOUT, null, true);
            UserLogged.Logout();

            App.SetCultureInfo(language);
            Application.Current.MainPage = new AppShell();
        }
    }
}
