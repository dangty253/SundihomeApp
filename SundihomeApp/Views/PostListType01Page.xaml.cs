using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class PostListType01Page : ContentPage
    {
        public PostListTypePageViewModel viewModel;
        public PostListPageViewModel viewModelType0;
        public PostListPageViewModel viewModelType1;
        private int CurrentIndex = 0;

        private LookUpControl LookUpControlProvince;
        private LookUpControl LookUpControlDistrict;
        private LookUpControl LookUpControlType;

        public PostListType01Page()
        {
            InitializeComponent();

            Segment.ItemsSource = new List<string>() { Language.can_ban, Language.cho_thue };
            Segment.SetActive(0);

            this.BindingContext = viewModel = new PostListTypePageViewModel();
            this.ListView0.BindingContext = viewModelType0 = new PostListPageViewModel(0);
            this.ListView1.BindingContext = viewModelType1 = new PostListPageViewModel(1);

            ListView0.ItemTemplate = new DataTemplate(typeof(Cells.PostViewCell));
            ListView1.ItemTemplate = new DataTemplate(typeof(Cells.PostViewCell));
            Init();

        }

        private async void Init()
        {
            ListView0.ItemTapped += Lv_ItemTapped;
            ListView1.ItemTapped += Lv_ItemTapped;
            await Task.WhenAll(viewModelType0.LoadData(),
                viewModelType1.LoadData());
            loadingPopup.IsVisible = false;
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            Shell.Current.Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        private async void Segment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentIndex = Segment.GetCurrentIndex();
            if (CurrentIndex == 0)
            {
                if (viewModelType0.filterModel.ProvinceId != viewModelType1.filterModel.ProvinceId || viewModelType0.filterModel.DistrictId != viewModelType1.filterModel.DistrictId || viewModelType0.filterModel.LoaiBatDongSanId != viewModelType1.filterModel.LoaiBatDongSanId)
                {
                    viewModelType0.filterModel.ProvinceId = viewModelType1.filterModel.ProvinceId;
                    viewModelType0.filterModel.DistrictId = viewModelType1.filterModel.DistrictId;
                    viewModelType0.filterModel.LoaiBatDongSanId = viewModelType1.filterModel.LoaiBatDongSanId;
                    await viewModelType0.LoadOnRefreshCommandAsync();
                }

                ListView0.IsVisible = true;
                ListView1.IsVisible = false;
            }
            else
            {
                if (viewModelType0.filterModel.ProvinceId != viewModelType1.filterModel.ProvinceId || viewModelType0.filterModel.DistrictId != viewModelType1.filterModel.DistrictId || viewModelType0.filterModel.LoaiBatDongSanId != viewModelType1.filterModel.LoaiBatDongSanId)
                {
                    viewModelType1.filterModel.ProvinceId = viewModelType0.filterModel.ProvinceId;
                    viewModelType1.filterModel.DistrictId = viewModelType0.filterModel.DistrictId;
                    viewModelType1.filterModel.LoaiBatDongSanId = viewModelType0.filterModel.LoaiBatDongSanId;
                    await viewModelType1.LoadOnRefreshCommandAsync();
                }

                ListView0.IsVisible = false;
                ListView1.IsVisible = true;
            }
        }


        public async void GoToMap_Clicked(object sender, EventArgs e)
        {
            var index = (short)CurrentIndex;
            FilterModel filterModel = new FilterModel();

            if (index == 0)
            {
                filterModel.PostType = 0;
            }
            else
            {
                filterModel.PostType = 1;
            }

            await Navigation.PushAsync(new MapsPage(filterModel));
        }

        public async void OpenSearch_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new FilterPage(CurrentIndex));
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

            if (CurrentIndex == 0)
            {
                if (viewModel.Province == null) viewModelType0.filterModel.ProvinceId = null;
                else viewModelType0.filterModel.ProvinceId = viewModel.Province.Id;
                viewModelType0.filterModel.DistrictId = null;

                await viewModelType0.LoadOnRefreshCommandAsync();
            }
            else
            {
                if (viewModel.Province == null) viewModelType1.filterModel.ProvinceId = null;
                else viewModelType1.filterModel.ProvinceId = viewModel.Province.Id;
                viewModelType1.filterModel.DistrictId = null;

                await viewModelType1.LoadOnRefreshCommandAsync();
            }
            LabelDistrict.Text = Language.quan_huyen;
            LabelDistrict.TextColor = Color.Black;
            await viewModel.GetDistrictAsync();
            viewModel.District = null;

            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            loadingPopup.IsVisible = true;

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
            if (CurrentIndex == 0)
            {
                if (viewModel.District == null) viewModelType0.filterModel.DistrictId = null;
                else viewModelType0.filterModel.DistrictId = viewModel.District.Id;

                await viewModelType0.LoadOnRefreshCommandAsync();
            }
            else
            {
                if (viewModel.District == null) viewModelType1.filterModel.DistrictId = null;
                else viewModelType1.filterModel.DistrictId = viewModel.District.Id;

                await viewModelType1.LoadOnRefreshCommandAsync();
            }

            loadingPopup.IsVisible = false;
        }
        public async void Type_Change(object sender, LookUpChangeEvent e)
        {
            loadingPopup.IsVisible = true;

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
            if (CurrentIndex == 0)
            {
                if (viewModel.Type == null) viewModelType0.filterModel.LoaiBatDongSanId = null;
                else viewModelType0.filterModel.LoaiBatDongSanId = (short?)viewModel.Type.Id;

                await viewModelType0.LoadOnRefreshCommandAsync();
            }
            else
            {
                if (viewModel.Type == null) viewModelType1.filterModel.LoaiBatDongSanId = null;
                else viewModelType1.filterModel.LoaiBatDongSanId = (short?)viewModel.Type.Id;

                await viewModelType1.LoadOnRefreshCommandAsync();
            }
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
            if (CurrentIndex == 0)
            {
                viewModelType0.filterModel.ProvinceId = null;
                viewModelType0.filterModel.DistrictId = null;
                viewModelType0.filterModel.LoaiBatDongSanId = null;
                await viewModelType0.LoadOnRefreshCommandAsync();
            }
            else
            {
                viewModelType1.filterModel.ProvinceId = null;
                viewModelType1.filterModel.DistrictId = null;
                viewModelType1.filterModel.LoaiBatDongSanId = null;
                await viewModelType1.LoadOnRefreshCommandAsync();
            }

            loadingPopup.IsVisible = false;
        }
    }
}
