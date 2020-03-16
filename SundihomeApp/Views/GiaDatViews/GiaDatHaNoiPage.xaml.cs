using System;
using System.Collections.Generic;
using SundihomeApp.ViewModels.GiaDatViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SundihomeApp.Views.GiaDatViews
{
    public partial class GiaDatHaNoiPage : ContentPage
    {
        public GiaDatHaNoiPageViewModel viewModel;
        public GiaDatHaNoiPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new GiaDatHaNoiPageViewModel();
            On<iOS>().SetUseSafeArea(true);
            Init();
        }
        public async void Init()
        {
            await viewModel.GetDistrictAsync();
            loadingPopup.IsVisible = false;
        }

        private async void District_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            this.viewModel.KhuDoThi = null;
            this.viewModel.GiaDatHaNoi = null;
            this.viewModel.GiaDatList.Clear();
            await this.viewModel.LoadKhuDoThiList(chkKhuDoThi.IsChecked);
            loadingPopup.IsVisible = false;

        }
        private async void KhuDoThi_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            await this.viewModel.LoaGiaDatHaNoiList(chkKhuDoThi.IsChecked);
            this.viewModel.GiaDatHaNoi = null;
            loadingPopup.IsVisible = false;
        }

        public async void OnStatusKhuDoThiCheckedTapped(object sender, EventArgs e)
        {
            chkStreet.IsChecked = false;
            chkKhuDoThi.IsChecked = true;
            loadingPopup.IsVisible = true;
            this.viewModel.KhuDoThi = null;
            this.viewModel.GiaDatHaNoi = null;
            await this.viewModel.LoadKhuDoThiList(chkKhuDoThi.IsChecked);
            viewModel.GiaDatList.Clear();
            loadingPopup.IsVisible = false;
        }

        public async void OnStatusStreetCheckedTapped(object sender, EventArgs e)
        {
            chkKhuDoThi.IsChecked = false;
            chkStreet.IsChecked = true;
            loadingPopup.IsVisible = true;
            this.viewModel.KhuDoThi = null;
            this.viewModel.GiaDatHaNoi = null;
            await this.viewModel.LoadKhuDoThiList(chkKhuDoThi.IsChecked);
            viewModel.GiaDatList.Clear();
            loadingPopup.IsVisible = false;
        }
    }
}
