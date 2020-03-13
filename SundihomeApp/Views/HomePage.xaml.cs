using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.Views.Furniture;
using SundihomeApp.Views.GiaDatViews;
using SundihomeApp.Views.LiquidationViews;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SundihomeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private readonly HomePageViewModel viewModel;

        public HomePage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new HomePageViewModel();


            Image_home_GiaDat.Source = ApiConfig.CloudStorageApiCDN + "/icon/home_GiaDat.jpg";
            Image_home_GoiVay.Source = ApiConfig.CloudStorageApiCDN + "/icon/home_GoiVay.jpg";
            Image_home_MoiGioi.Source = ApiConfig.CloudStorageApiCDN + "/icon/home_MoiGioi.jpg";

            this.Init();
        }

        public async void Init()
        {
            await Task.WhenAll(viewModel.LoadNewestBuyOrRentList(), viewModel.LoadNewestNeedtoBuyOrRentList(), viewModel.LoadProjectList(), viewModel.LoadNewFurnitureProducts(), viewModel.LoadLiquidationList());
            MessagingCenter.Subscribe<AddLiquidationPage>(this, "OnSaveItem", async (sender) => await viewModel.LoadLiquidationList());
            MessagingCenter.Subscribe<LiquidationDetailPage, Guid>(this, "OnDeleted", async (sender, liquidationId) =>
            {
                if (viewModel.Liquidations.Any(x => x.Id == liquidationId))
                {
                    await viewModel.LoadLiquidationList();
                }
            });
            loadingPopup.IsVisible = false;
        }

        public async void ProjectListView_Tapped(object sender, EventArgs e)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.PROJECT_INDEX];

            Guid projectId = (Guid)(((Frame)sender).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await appShell.CurrentItem.Navigation.PushAsync(new ProjectDetailPage(projectId));
        }

        public async void GoToNotifications_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new NotificationsPage());
        }

        public async void OpenSearch_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new FilterPage());
        }

        public async void ViewMoreType01_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//batdongsan/ban_chothue");
        }
        public async void ViewMoreType23_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//batdongsan/canmua_thue");
        }
        public async void ViewMoreProject_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProjectListPage());
        }
        public async void GoTo_PostDetail_Cliked(object sender, EventArgs e)
        {
            var stacklayout = sender as StackLayout;
            var tap = stacklayout.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;
            await Shell.Current.Navigation.PushAsync(new PostDetailPage(id));
        }

        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged == false)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new PostPage(0));
            }
        }
        private async void GoTo_FurnitureProductDetail_Cliked(object sender, EventArgs e)
        {
            var stacklayout = sender as StackLayout;
            var tap = stacklayout.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;

            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.FURNITURE_INDEX];
            await appShell.CurrentItem.Navigation.PushAsync(new ProductDetailPage(id));
        }


        private void ViewAllFurnitureProduct_Clicked(object sender, EventArgs e)
        {
            AppShell appShell = (AppShell)Shell.Current;
            appShell.CurrentItem = appShell.Items[AppShell.FURNITURE_INDEX];
        }

        private async void OnLiquidationDetail_Tapped(object sender, EventArgs e)
        {
            Grid stack = sender as Grid;
            TapGestureRecognizer tap = stack.GestureRecognizers[0] as TapGestureRecognizer;
            Liquidation item = tap.CommandParameter as Liquidation;

            await Shell.Current.GoToAsync("//LiquidationListPage");
            await Shell.Current.Navigation.PushAsync(new LiquidationDetailPage(item.Id));
        }
        private async void ViewAll_Liquidation_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LiquidationListPage");
        }
        public async void AddNeedPost_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged == false)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new PostPage(2));
            }
        }

        private void ContentPage_SizeChanged(object sender, System.EventArgs e)
        {
            double height = (this.Width / 2) - 30;
            sectionGiaDatLeft.HeightRequest = height;
            sectionMoigioRightTop.HeightRequest = sectionGoiVayRightBototm.HeightRequest = (height - 10) / 2;
        }

        private async void GiaDat_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new GiaDatListPage());
        }

        private async void MoiGioi_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//batdongsan/agents?index=0");
        }

        private async void GoiVay_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//batdongsan/agents?index=1");
        }
    }
}
