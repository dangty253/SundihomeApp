using System;
using System.Collections.Generic;
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
    public partial class AgentContentView : ContentView
    {
        private readonly AllListPageViewModel viewModel;
        private LookUpControl LookUpControlProvince;
        private LookUpControl LookUpControlDistrict;
        private LookUpControl LookUpControlType;

        public AgentContentView()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AllListPageViewModel();
            Init();
        }
        public async void Init()
        {
            DataList.ItemTapped += DataList_ItemTapped;
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
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

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {

            loadingPopup.IsVisible = true;
            if (viewModel.Province.Id == -1)
            {
                LabelProvince.Text = Language.tinh_thanh;
                LabelProvince.TextColor = Color.Black;
                viewModel.Province = null;
            }
            else
            {
                LabelProvince.Text = viewModel.Province.Name;
                LabelProvince.TextColor = Color.FromHex("#026294");
            }

            LabelDistrict.Text = Language.quan_huyen;
            await viewModel.GetDistrictAsync();
            viewModel.District = null;
            LabelDistrict.TextColor = Color.Black;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.District.Id == -1)
            {
                LabelDistrict.Text = Language.quan_huyen;
                LabelDistrict.TextColor = Color.Black;
                viewModel.District = null;
            }
            else
            {
                LabelDistrict.Text = viewModel.District.Name;
                LabelDistrict.TextColor = Color.FromHex("#026294");
            }
            loadingPopup.IsVisible = true;

            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }
        public async void Type_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.Type.Id == -1)
            {
                LabelType.Text = Language.loai_bat_dong_san;
                LabelType.TextColor = Color.Black;
                viewModel.Type = null;
            }
            else
            {
                LabelType.Text = viewModel.Type.Name;
                LabelType.TextColor = Color.FromHex("#026294");
            }
            loadingPopup.IsVisible = true;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void FilterProvince_Click(object sender, EventArgs e)
        {
            if (LookUpControlProvince == null)
            {
                await viewModel.GetProvinceAsync();
                LookUpControlProvince = new LookUpControl();
                LookUpControlProvince.ItemsSource = viewModel.ProvinceList;
                LookUpControlProvince.SelectedItemChange += Province_Change;
                LookUpControlProvince.BottomModal = LookUpModal;
                LookUpControlProvince.NameDisplay = "Name";
                LookUpControlProvince.Placeholder = Language.tinh_thanh;
                LookUpControlProvince.SetBinding(LookUpControl.SelectedItemProperty, new Binding("Province") { Source = viewModel });
            }

            await LookUpControlProvince.OpenModal();
        }
        public async void FilterDistric_Click(object sender, EventArgs e)
        {
            if (LookUpControlDistrict == null)
            {
                LookUpControlDistrict = new LookUpControl();
                LookUpControlDistrict.ItemsSource = viewModel.DistrictList;
                LookUpControlDistrict.SelectedItemChange += District_Change;
                LookUpControlDistrict.BottomModal = LookUpModal;
                LookUpControlDistrict.NameDisplay = "Name";
                LookUpControlDistrict.Placeholder = Language.quan_huyen;
                LookUpControlDistrict.SetBinding(LookUpControl.SelectedItemProperty, new Binding("District") { Source = viewModel });
            }
            await LookUpControlDistrict.OpenModal();
        }
        public async void FilterType_Click(object sender, EventArgs e)
        {
            if (LookUpControlType == null)
            {
                LookUpControlType = new LookUpControl();
                LookUpControlType.ItemsSource = viewModel.TypeList;
                LookUpControlType.SelectedItemChange += Type_Change;
                LookUpControlType.BottomModal = LookUpModal;
                LookUpControlType.NameDisplay = "Name";
                LookUpControlType.Placeholder = Language.loai_bat_dong_san;
                LookUpControlType.SetBinding(LookUpControl.SelectedItemProperty, new Binding("Type") { Source = viewModel });
            }
            await LookUpControlType.OpenModal();
        }

        public async void Clear_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            LabelProvince.Text = Language.tinh_thanh;
            LabelDistrict.Text = Language.quan_huyen;
            LabelType.Text = Language.loai_bat_dong_san;
            LabelProvince.TextColor = Color.Black;
            LabelDistrict.TextColor = Color.Black;
            LabelType.TextColor = Color.Black;
            viewModel.Province = null;
            viewModel.District = null;
            viewModel.Type = null;
            viewModel.DistrictList.Clear();

            await viewModel.LoadOnRefreshCommandAsync();



            loadingPopup.IsVisible = false;
        }
    }
}
