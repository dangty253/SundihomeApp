using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class MyToDayListPage : ContentPage
    {
        public LiquidationToDayFilterViewModel viewModel;
        public MyToDayListPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new LiquidationToDayFilterViewModel();
            viewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
            Init();
        }

        public async void Init()
        {
            MessagingCenter.Subscribe<AddToDayPage>(this, "OnSaveItem", (sender) => viewModel.RefreshCommand.Execute(null));
            MessagingCenter.Subscribe<ToDayDetailPage, Guid>(this, "OnDeleted", (sender, id) =>
            {
                if (this.viewModel.Data.Any(x => x.Id == id))
                {
                    var removeItem = this.viewModel.Data.Single(x => x.Id == id);
                    this.viewModel.Data.Remove(removeItem);
                }
            });
            MessagingCenter.Subscribe<PickerLiquidationPage>(this, "OnSaveItem", (sender) => viewModel.RefreshCommand.Execute(null));

            ListViewThanhLy.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as LiquidationToDay;
                await Navigation.PushAsync(new ToDayDetailPage(item.Id));
            };
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void FilterByStatus_Clicked(object sender, EventArgs e)
        {
            RadBorder radBoder = sender as RadBorder;
            if (radBoder.BackgroundColor == Color.FromHex("#eeeeee")) return;

            TapGestureRecognizer tapGestureRecognizer = radBoder.GestureRecognizers[0] as TapGestureRecognizer;

            if (tapGestureRecognizer.CommandParameter != null)
            {
                int Status = int.Parse(tapGestureRecognizer.CommandParameter.ToString());
                viewModel.FilterModel.Status = Status;
            }
            else
            {
                viewModel.FilterModel.Status = null;
            }
            loadingPopup.IsVisible = true;


            // inactive
            foreach (RadBorder item in StackLayoutFilter.Children)
            {
                item.BorderColor = Color.FromHex("#eeeeee");
                item.BackgroundColor = Color.White;
            }
            // active.
            radBoder.BackgroundColor = Color.FromHex("#eeeeee");
            radBoder.BorderColor = Color.FromHex("#aaaaaa");



            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void AddLiquidation_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged)
            {
                var answer = await DisplayActionSheet(Language.dang_san_pham_trong_ngay, Language.huy, null, Language.chon_tu_san_pham_thanh_ly, Language.them_san_pham);
                if (answer == Language.chon_tu_san_pham_thanh_ly)
                {
                    await Navigation.PushAsync(new PickerLiquidationPage(true));
                }
                if (answer == Language.them_san_pham)
                {
                    await Navigation.PushAsync(new AddToDayPage());
                }
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_dang_ky_de_duoc_dang_san_pham, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }

        }
        public void Clicked_BtnSearch(object sender, EventArgs e)
        {
            viewModel.FilterModel.Keyword = MySearchBarToDayListPage.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChaned(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MySearchBarToDayListPage.Text))
            {
                if (!string.IsNullOrWhiteSpace(viewModel.FilterModel.Keyword))
                {
                    viewModel.FilterModel.Keyword = null;
                    viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
