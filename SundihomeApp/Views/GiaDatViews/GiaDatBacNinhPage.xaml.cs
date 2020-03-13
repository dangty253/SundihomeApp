using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApp.ViewModels.GiaDatViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SundihomeApp.Views.GiaDatViews
{
    public partial class GiaDatBacNinhPage : ContentPage
    {
        public GiaDatBacNinhPageViewModel viewModel;
        public GiaDatBacNinhPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new GiaDatBacNinhPageViewModel();
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
            this.viewModel.KhuDanCu = null;
            this.viewModel.GiaDat = null;
            this.viewModel.KhuVuc = null;
            
            if (viewModel.District?.Id == 256)
            {
                this.viewModel.IsNoCity = false;
                await this.viewModel.LoadKhuDanCuList(chkKhuDanCu.IsChecked);
            }
            else
            {

                this.viewModel.IsNoCity = true;
                await this.viewModel.LoadKhuDanCuList(chkKhuDanCu.IsChecked);
                await viewModel.LoadWards();
            }
            loadingPopup.IsVisible = false;

        }
        private async void Ward_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            this.viewModel.KhuDanCu = null;
            this.viewModel.GiaDat = null;
            this.viewModel.KhuVuc = null;
            loadingPopup.IsVisible = true;
            await Task.WhenAll(this.viewModel.LoadKhuDanCuList(chkKhuDanCu.IsChecked), this.viewModel.LoadKhuVucList());
            this.viewModel.GiaDatList.Clear();
            loadingPopup.IsVisible = false;
        }
        private async void KhuDanCu_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            await this.viewModel.LoaGiaDatList(chkKhuDanCu.IsChecked);
            this.viewModel.GiaDat = null;
            loadingPopup.IsVisible = false;
        }

        public async void OnStatusKhuDanCuCheckedTapped(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            chkStreet.IsChecked = false;
            chkKhuDanCu.IsChecked = true;
            this.viewModel.KhuDanCu = null;
            this.viewModel.GiaDat = null;
            this.viewModel.KhuVuc = null;
            await this.viewModel.LoadKhuDanCuList(chkKhuDanCu.IsChecked);
            viewModel.GiaDatList.Clear();
            loadingPopup.IsVisible = false;
        }

        public async void OnStatusStreetCheckedTapped(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;

            chkKhuDanCu.IsChecked = false;
            chkStreet.IsChecked = true;
            this.viewModel.KhuDanCu = null;
            this.viewModel.GiaDat = null;
            this.viewModel.KhuVuc = null;
            await this.viewModel.LoadKhuDanCuList(chkKhuDanCu.IsChecked);
            viewModel.GiaDatList.Clear();
            loadingPopup.IsVisible = false;
        }
    }
}