using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class CompanyPostListContentView : ContentView
    {
        public ListViewPageViewModel2<Post> viewModel;
        public string Keyword;
        public CompanyPostListContentView(Guid companyId)
        {
            InitializeComponent();

            this.BindingContext = viewModel = new ListViewPageViewModel2<Post>()
            {
                PreLoadData = new Command(() =>
                {
                    string url = $"{ApiRouter.POST_GETBYCOMPANY}/{companyId}?page={viewModel.Page}";
                    if (!string.IsNullOrWhiteSpace(Keyword))
                    {
                        url += $"&keyword={Keyword}";
                    }
                    viewModel.ApiUrl = url;
                })
            };

            Init();
        }

        public async void Init()
        {
            MessagingCenter.Subscribe<PostPage, Guid>(this, "OnDeleteSuccess", async (sender, PostId) =>
            {
                var deletedPost = this.viewModel.Data.SingleOrDefault(x => x.Id == PostId);
                if (deletedPost != null)
                {
                    this.viewModel.Data.Remove(deletedPost);
                }
            });

            MessagingCenter.Subscribe<PostPage>(this, "OnSavePost", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });

            LvData.ItemTapped += LvData_ItemTapped;
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            await Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        public void Search_Pressed(object sender, EventArgs e)
        {
            Keyword = searchBar.Text;
            viewModel.RefreshCommand.Execute(null);
        }

        public void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text) && !string.IsNullOrWhiteSpace(Keyword))
            {
                Keyword = null;
                viewModel.RefreshCommand.Execute(null);
            }
        }

    }
}
