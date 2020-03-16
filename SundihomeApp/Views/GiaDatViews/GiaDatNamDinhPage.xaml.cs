using System;
using System.Collections.Generic;
using SundihomeApp.Controls;
using SundihomeApp.ViewModels.GiaDatViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace SundihomeApp.Views.GiaDatViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GiaDatNamDinhPage : ContentPage
    {
        public GiaDatNamDinhPageViewModel viewModel;
        public GiaDatNamDinhPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new GiaDatNamDinhPageViewModel();
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
            this.viewModel.Street = null;
            this.viewModel.StreetDistance = null;
            this.viewModel.StreetDistances.Clear();
            this.viewModel.Ward = null;
            if (viewModel.District?.Id == 356)
            {
                await this.viewModel.LoadStreets();
                this.viewModel.IsCity = false;
            }
            else
            {

                this.viewModel.IsCity = true;
                viewModel.Streets.Clear();
                this.viewModel.StreetDistances.Clear();
                await viewModel.LoadWards();
            }
            loadingPopup.IsVisible = false;
        }

        private async void Ward_Changed(object sender, EventArgs e)
        {
            this.viewModel.Street = null;
            this.viewModel.StreetDistance = null;
            loadingPopup.IsVisible = true;
            await this.viewModel.LoadStreets();
            this.viewModel.StreetDistances.Clear();
            loadingPopup.IsVisible = false;
        }

        private async void Street_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            this.viewModel.StreetDistances.Clear();
            await this.viewModel.LoadStreetDistances();
            if (this.viewModel.StreetDistances.Count > 0)
            {
                this.viewModel.StreetDistance = this.viewModel.StreetDistances[0];
            }
            else
            {
                this.viewModel.StreetDistance = null;
            }
            
            loadingPopup.IsVisible = false;
        }
    }
}
