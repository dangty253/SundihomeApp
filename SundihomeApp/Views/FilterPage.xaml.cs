using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class FilterPage : ContentPage
    {
        private readonly FilterPageViewModel viewModel;
        public FilterPage(int? SelectedIndex = null)
        {
            InitializeComponent();
            segmentPostType.ItemsSource = new string[] { Language.can_ban, Language.cho_thue, Language.can_mua, Language.can_thue };
            this.BindingContext = viewModel = new FilterPageViewModel();
            if (SelectedIndex.HasValue)
            {
                segmentPostType.SelectedIndex = SelectedIndex.Value;
            }

            Init();
            if (Device.RuntimePlatform == Device.Android)
            {
                BackgroundColor = Color.White;
            }
        }

        public async void Init()
        {
            await Task.WhenAll(viewModel.GetProvinceAsync(),
             viewModel.GetProjectsAsync());
            loadingPopup.IsVisible = false;
        }


        public async void Filter_Clicked(object sender, EventArgs e)
        {
            FilterModel filterModel = new FilterModel();
            if (segmentPostType.SelectedIndex != -1)
            {
                filterModel.PostType = (short)segmentPostType.SelectedIndex;
            }

            if (viewModel.LoaiBatDongSan != null)
            {
                filterModel.LoaiBatDongSanId = viewModel.LoaiBatDongSan.Id;
            }

            if (viewModel.Project != null)
            {
                filterModel.ProjectId = viewModel.Project.Id;
            }

            if (viewModel.Province != null)
            {
                filterModel.ProvinceId = viewModel.Province.Id;
            }

            if (viewModel.District != null)
            {
                filterModel.DistrictId = viewModel.District.Id;
            }

            if (viewModel.Ward != null)
            {
                filterModel.WardId = viewModel.Ward.Id;
            }

            if (viewModel.Area != null)
            {
                if (viewModel.Area.ValueFrom.HasValue)
                {
                    filterModel.AreaFrom = viewModel.Area.ValueFrom.Value;
                }
                if (viewModel.Area.ValueTo.HasValue)
                {
                    filterModel.AreaTo = viewModel.Area.ValueTo.Value;
                }
            }
            if (viewModel.PriceFrom != null)
            {
                filterModel.PriceFrom = viewModel.PriceFrom.Value.Value;
            }
            if (viewModel.PriceTo != null)
            {
                filterModel.PriceTo = viewModel.PriceTo.Value.Value;
            }
            if (viewModel.SoPhongNgu.HasValue)
            {
                filterModel.SoPhongNgu = viewModel.SoPhongNgu.Value;
            }
            if (viewModel.SoPhongTam.HasValue)
            {
                filterModel.SoPhongTam = viewModel.SoPhongTam.Value;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Keyword))
            {
                filterModel.Keyword = viewModel.Keyword.Trim();
            }

            await Shell.Current.Navigation.PushAsync(new SearchResultPage(filterModel));
        }

        public async void GoToMap_Clicked(object sender, EventArgs e)
        {
            FilterModel filterModel = new FilterModel();
            if (segmentPostType.SelectedIndex != -1)
            {
                filterModel.PostType = (short)segmentPostType.SelectedIndex;
            }

            if (viewModel.LoaiBatDongSan != null)
            {
                filterModel.LoaiBatDongSanId = viewModel.LoaiBatDongSan.Id;
            }

            if (viewModel.Project != null)
            {
                filterModel.ProjectId = viewModel.Project.Id;
            }

            if (viewModel.Province != null)
            {
                filterModel.ProvinceId = viewModel.Province.Id;
            }

            if (viewModel.District != null)
            {
                filterModel.DistrictId = viewModel.District.Id;
            }

            if (viewModel.Ward != null)
            {
                filterModel.WardId = viewModel.Ward.Id;
            }

            if (viewModel.Area != null)
            {
                if (viewModel.Area.ValueFrom.HasValue)
                {
                    filterModel.AreaFrom = viewModel.Area.ValueFrom.Value;
                }
                if (viewModel.Area.ValueTo.HasValue)
                {
                    filterModel.AreaTo = viewModel.Area.ValueTo.Value;
                }
            }
            if (viewModel.PriceFrom != null)
            {
                filterModel.PriceFrom = viewModel.PriceFrom.Value.Value;
            }
            if (viewModel.PriceTo != null)
            {
                filterModel.PriceTo = viewModel.PriceTo.Value.Value;
            }
            if (viewModel.SoPhongNgu.HasValue)
            {
                filterModel.SoPhongNgu = viewModel.SoPhongNgu.Value;
            }
            if (viewModel.SoPhongTam.HasValue)
            {
                filterModel.SoPhongTam = viewModel.SoPhongTam.Value;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Keyword))
            {
                filterModel.Keyword = viewModel.Keyword.Trim();
            }

            await Shell.Current.Navigation.PushAsync(new MapsPage(filterModel));
        }

        public void ResetForm_Clicked(object sender, EventArgs e)
        {
            segmentPostType.SelectedIndex = -1;
            viewModel.Project = null;
            viewModel.LoaiBatDongSan = null;
            viewModel.Province = null;
            viewModel.PriceFrom = null;
            viewModel.PriceTo = null;
            viewModel.Area = null;
            viewModel.SoPhongNgu = null;
            viewModel.SoPhongTam = null;
            viewModel.Keyword = null;
        }
        public async void OnProvice_Change(object sender, LookUpChangeEvent e)
        {
            viewModel.District = null;
        }

        public void OnDistrict_Change(object sender, LookUpChangeEvent e)
        {
            viewModel.Ward = null;
        }
    }
}
