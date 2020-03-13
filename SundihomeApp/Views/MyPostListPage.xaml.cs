using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class MyPostListPage : ContentPage
    {
        public SearchPageResultViewModel viewModel;
        public MyPostListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new SearchPageResultViewModel();
            viewModel.FilterModel = new FilterModel();
            viewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
            Init();
        }

        private async void Init()
        {
            lv.ItemTapped += Lv_ItemTapped;
            await viewModel.LoadData();

            MessagingCenter.Subscribe<PostDetailPage, Guid>(this, "OnDeleteSuccess", async (sender, PostId) =>
            {
                loadingPopup.IsVisible = true;
                if (this.viewModel.Data.Any(x => x.Id == PostId))
                {
                    var deletedPost = this.viewModel.Data.Single(x => x.Id == PostId);
                    this.viewModel.Data.Remove(deletedPost);
                }
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<PostPage>(this, "OnSavePost", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });

            loadingPopup.IsVisible = false;
        }

        private async void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            await Navigation.PushAsync(new PostDetailPage(post.Id));
        }
        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new PostPage());
        }

        private async void Search_Pressed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            var text = searchBar.Text;
            viewModel.FilterModel.Keyword = text;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }
        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == null || e.NewTextValue == "")
            {
                if (!string.IsNullOrWhiteSpace(viewModel.FilterModel.Keyword))
                {
                    viewModel.FilterModel.Keyword = null;
                    viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
