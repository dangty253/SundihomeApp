using System;
using System.Collections.Generic;
using SundihomeApp.ViewModels.GiaDatViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.GiaDatViews
{
    public partial class GiaDatHaNamPage : ContentPage
    {
        public GiaDatHaNamPageViewModel viewModel;
        public GiaDatHaNamPage()
        {
            InitializeComponent();
            Init();
        }
        public async void Init()
        {
            this.BindingContext = viewModel = new GiaDatHaNamPageViewModel();
            await viewModel.GetDistrictAsync();
            loadingPopup.IsVisible = false;
        }
        public async void District_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            this.viewModel.Ward = null;
            await viewModel.GetWardAsync();
            loadingPopup.IsVisible = false;
        }
    }
}
