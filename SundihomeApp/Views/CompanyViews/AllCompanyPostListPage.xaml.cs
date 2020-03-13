using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class AllCompanyPostListPage : ContentPage
    {
        private ListViewPageViewModel2<Post> viewModel;
        public AllCompanyPostListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewPageViewModel2<Post>()
            {
                PreLoadData = new Command(() => viewModel.ApiUrl = $"{ApiRouter.POST_GETBYALLCOMPANIES}?page={viewModel.Page}")
            };
            Init();
        }
        public async void Init()
        {
            LvData.ItemTapped += LvData_ItemTapped;
            MessagingCenter.Subscribe<PostPage, Guid>(this, "OnDeleteSuccess", async (sender, PostId) =>
            {
                var deletedPost = this.viewModel.Data.SingleOrDefault(x => x.Id == PostId);
                if (deletedPost != null)
                    this.viewModel.Data.Remove(deletedPost);
            });
            MessagingCenter.Subscribe<PostPage>(this, "OnSavePost", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            await Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }

            if (UserLogged.RoleId == 0)
            {
                await Shell.Current.Navigation.PushAsync(new PostPage(0, Guid.Parse(UserLogged.CompanyId)));
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new PostPage(0));
            }
        }
    }
}
