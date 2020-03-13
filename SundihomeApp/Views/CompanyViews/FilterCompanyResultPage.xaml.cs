using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class FilterCompanyResultPage : ContentPage
    {
        public FilterCompanyViewModel viewModel;
        public FilterCompanyResultPage(FilterCompanyModel filterCompanyModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new FilterCompanyViewModel(filterCompanyModel);
            Init();
        }
        public async void Init()
        {
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }
        public void click_OnCompany_GoDetail(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            var tap = grid.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;
            Shell.Current.Navigation.PushAsync(new CompanyProfileDetailPage(id));
        }
    }
}
