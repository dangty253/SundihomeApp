using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApp.Controls;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using SundihomeApp.Views.MoiGioiViews;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class AgentContentView : AbsoluteLayout
    {
        private LoadingPopup loadingPopup;
        private readonly AllListPageViewModel viewModel;
        public AgentContentView(LoadingPopup LoadingPopup)
        {
            InitializeComponent();
            this.loadingPopup = LoadingPopup;
            this.BindingContext = viewModel = new AllListPageViewModel();
            Init();
        }

        public AgentContentView()
        {
            InitializeComponent();

            loadingPopup = new LoadingPopup()
            {
                IsVisible = true
            };

            this.Children.Add(loadingPopup);

            this.BindingContext = viewModel = new AllListPageViewModel();
            Init();
        }

        public async void Init()
        {
            DataList.ItemTapped += DataList_ItemTapped;
            await Task.WhenAll(viewModel.LoadData(),
                viewModel.GetProvinceAsync());
            ShowButtonDKMoiGioi();
            loadingPopup.IsVisible = false;
        }

        private void ShowButtonDKMoiGioi()
        {
            // chua dnag nhap hoac dang nhap roi ma chua phai la moi gioi
            if (UserLogged.IsLogged == false || (UserLogged.IsLogged && UserLogged.Type == 0))
            {
                FrameBtnDangKyMoiGioi.IsVisible = true;
                BtnDangKyMoiGioi.Clicked += DangKyMoiGioi_Clicked;
            }
        }

        private async void DataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var moigioi = e.Item as MoiGioi;
            await Navigation.PushAsync(new ThongTinMoiGioiPage(moigioi.Id, false));
        }

        public void Clicked_BtnSearch(object sender, EventArgs e)
        {
            viewModel.Keyword = SearchBarMoiGgioi.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChaned(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBarMoiGgioi.Text))
            {
                if (!string.IsNullOrWhiteSpace(viewModel.Keyword))
                {
                    viewModel.Keyword = null;
                    viewModel.RefreshCommand.Execute(null);
                }
            }
        }

        private async void DangKyMoiGioi_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_de_dang_ky_moi_gioi, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            if (UserLogged.Type == 1)
            {
                await Shell.Current.DisplayAlert("", Language.ban_dang_la_moi_gioi, Language.dong);
            }
            else
            {
                if (ModalDangKyMoiGioi.Body == null)
                {
                    var dangKyMoiGioiContentView = new DangKyMoiGioiContentView(LookUpModal, Guid.Parse(UserLogged.Id));
                    dangKyMoiGioiContentView.OnSaved += async (object s, EventArgs e2) =>
                    {
                        FrameBtnDangKyMoiGioi.IsVisible = false;
                        await ModalDangKyMoiGioi.Hide();
                        await Shell.Current.GoToAsync("//quanlymoigioi");
                    };
                    dangKyMoiGioiContentView.OnCancel += async (object ssender, EventArgs e2) => await ModalDangKyMoiGioi.Hide();
                    ModalDangKyMoiGioi.Body = dangKyMoiGioiContentView;
                    ModalDangKyMoiGioi.CustomCloseButton(dangKyMoiGioiContentView.Cancel_Clicked);
                }
                await ModalDangKyMoiGioi.Show();
            }
        }


        public async void FilterDistrict_Changed(object sender, LookUpChangeEvent e)
        {
            loadingPopup.IsVisible = true;
            if (viewModel.District.Id == -1)
            {
                viewModel.District = null;
            }
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void FilterType_Changed(object sender, LookUpChangeEvent e)
        {
            if (viewModel.Type.Id == -1)
            {
                viewModel.Type = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void FilterProvince_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (viewModel.Province.Id == -1)
            {
                viewModel.Province = null;
            }
            await viewModel.GetDistrictAsync();
            viewModel.District = null;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void Clear_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            viewModel.Province = null;
            viewModel.District = null;
            viewModel.Type = null;
            viewModel.DistrictList.Clear();

            await viewModel.LoadOnRefreshCommandAsync();



            loadingPopup.IsVisible = false;
        }
    }
}
