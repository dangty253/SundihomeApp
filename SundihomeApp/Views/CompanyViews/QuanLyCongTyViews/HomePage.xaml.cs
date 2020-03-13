using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using SundihomeApp.Views.MoiGioiViews;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews.QuanLyCongTyViews
{
    public partial class HomePage : ContentPage
    {
        public HomePageViewModel viewModel;
        public HomePage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new HomePageViewModel();
            Init();
        }
        public async void Init()
        {
            await Task.WhenAll(viewModel.LoadNewestBuyOrRentList(), viewModel.GetCompany());

            if (UserLogged.RoleId == 0)
            {
                await viewModel.LoadContactNeeds();
                StackTitlNhuCauKhachHang.IsVisible = true;
                StackListNhuCauKhachHang.IsVisible = true;
            }

            if (viewModel.Company.CreatedById == Guid.Parse(UserLogged.Id))
            {
                StackButtonGroup.IsVisible = true;
            }

            MessagingCenter.Subscribe<ViewModels.MoiGioiViewModels.AddContactNeedContentViewModel>(this, "ReloadNhuCauList", async (sender) =>
            {
                await this.viewModel.LoadContactNeeds();
            });

            MessagingCenter.Subscribe<PostPage, Guid>(this, "OnDeleteSuccess", async (sender, PostId) =>
            {
                var deletedPost = this.viewModel.NewestBuyOrRentList.SingleOrDefault(x => x.Id == PostId);
                if (deletedPost != null)
                    this.viewModel.NewestBuyOrRentList.Remove(deletedPost);
            });
            MessagingCenter.Subscribe<PostPage>(this, "OnSavePost", async (sender) =>
            {
                await viewModel.LoadNewestBuyOrRentList();
            });

            MessagingCenter.Subscribe<ContactListPage, Guid>(this, "DeleteContact", async (sender, id) =>
            {
                await this.viewModel.LoadContactNeeds();
            });

            loadingPopup.IsVisible = false;
        }


        public async void ViewMoreType01_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//quanlycongty/sanpham_congty");
        }
        public async void GoTo_PostDetail_Cliked(object sender, EventArgs e)
        {
            var stacklayout = sender as StackLayout;
            var tap = stacklayout.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;
            await Shell.Current.Navigation.PushAsync(new PostDetailPage(id));
        }

        private async void ViewContact_Tapped(object sender, EventArgs e)
        {
            var id = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await Navigation.PushAsync(new ContactDetailPage(id, true));
        }

        private async void ViewAllContactNeeds_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//quanlycongty/quanlykhachhang");
        }

        private async void EditCompany_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCompanyPage(viewModel.Company.Id));
        }
    }
}
