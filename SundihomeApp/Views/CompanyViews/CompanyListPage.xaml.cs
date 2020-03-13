using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class CompanyListPage : ContentPage
    {
        public CompanyListPageViewModel viewModel;
        public CompanyListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new CompanyListPageViewModel();
            Inti();
        }
        async void Inti()
        {
            await viewModel.LoadData();
            if (UserLogged.IsLogged && !string.IsNullOrWhiteSpace(UserLogged.CompanyId))
            {
                stAddCompany.IsVisible = false;
            }

            MessagingCenter.Subscribe<AddCompanyPage, Guid>(this, "OnDeleteSuccess", async (sender, CompanyId) =>
            {
                loadingPopup.IsVisible = true;
                if (this.viewModel.Data.Any(x => x.Id == CompanyId))
                {
                    var deleteCompany = this.viewModel.Data.Single(x => x.Id == CompanyId);
                    this.viewModel.Data.Remove(deleteCompany);
                }
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<AddCompanyPage>(this, "OnSaveCompany", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
                stAddCompany.IsVisible = false;
            });

            List<LoaiCongTyModel> listLoaiCongTyModel = new List<LoaiCongTyModel>(LoaiCongTyData.GetListNganhNghe());
            List<LoaiCongTyModel> newListLoaiCongTyModel = new List<LoaiCongTyModel>();
            newListLoaiCongTyModel.Add(new LoaiCongTyModel(-1, Language.tat_ca));
            foreach (var item in listLoaiCongTyModel)
            {
                newListLoaiCongTyModel.Add(item);
            }
            BindableLayout.SetItemsSource(stListLoaiCongty, newListLoaiCongTyModel);

            //set mau cho filter "Tat ca"
            var radBorder = stListLoaiCongty.Children[0] as RadBorder;
            radBorder.BackgroundColor = (Color)App.Current.Resources["MainDarkColor"];
            (radBorder.Content as Label).TextColor = Color.White;

            loadingPopup.IsVisible = false;
        }
        public void click_OnCompany_GoDetail(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            var tap = grid.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;
            Shell.Current.Navigation.PushAsync(new CompanyProfileDetailPage(id));
        }

        public async void click_AddCompany(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged && string.IsNullOrWhiteSpace(UserLogged.CompanyId))
            {
                await Shell.Current.Navigation.PushAsync(new AddCompanyPage());
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_dang_ky_de_tao_cong_ty, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
        }

        public async void Search_Clicked(object sender, EventArgs e)
        {
            FilterCompanyModel filterCompany = new FilterCompanyModel();
            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                filterCompany.keyword = searchBar.Text.Trim();
                await Shell.Current.Navigation.PushAsync(new FilterCompanyResultPage(filterCompany));
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_tu_khoa , Language.dong);
            }
        }

        public async void LoaiConty_Tapped(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            var radBorder = sender as RadBorder;
            TapGestureRecognizer click = radBorder.GestureRecognizers[0] as TapGestureRecognizer;
            short? id = (short)click.CommandParameter;

            //set mau cho filter
            Color MainDarkColor = (Color)App.Current.Resources["MainDarkColor"];
            IDictionary<int?, Color> color = new Dictionary<int?, Color>()
            {
                {id,MainDarkColor }
            };

            //set mau cho cac filter khong click
            var ortherRadBorder = stListLoaiCongty.Children.Where(x => x != radBorder);
            foreach (RadBorder item in ortherRadBorder)
            {
                item.BackgroundColor = Color.White;
                (item.Content as Label).TextColor = Color.FromHex("#444444");
            }

            //set mau cho filter dang chon
            radBorder.BackgroundColor = color[id];
            (radBorder.Content as Label).TextColor = Color.White;

            if (id == -1)
            {
                viewModel.filterCompanyModel.typeCompany = null;
                await viewModel.LoadOnRefreshCommandAsync();
            }
            else
            {
                viewModel.filterCompanyModel.typeCompany = id;
                await viewModel.LoadOnRefreshCommandAsync();
            }
            loadingPopup.IsVisible = false;
        }
    }
}
