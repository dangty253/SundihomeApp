using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class FilterEmployeePostListView : ContentView
    {
        private ListViewPageViewModel2<Post> viewModel;
        public string KeyWord { get; set; }
        public FilterEmployeePostListView(bool isCommitment, bool isCompany = false, string keyword = null)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewPageViewModel2<Post>();
            this.KeyWord = keyword;
            string BaseUrl = string.Empty;
            if (isCompany)
            {
                BaseUrl = $"{ApiRouter.COMPANY_GETPOSTLIST}?CompanyId={UserLogged.CompanyId}&Status=2&isCommitment={isCommitment}";
            }
            else
            {
                BaseUrl = $"{ApiRouter.EMPLOYEE_GETMYPOSTLIST}?isCommitment={isCommitment}";
            }

            viewModel.PreLoadData = new Command(() =>
            {
                string URL = $"{BaseUrl}&page={viewModel.Page}";
                if (!string.IsNullOrWhiteSpace(KeyWord))
                {
                    URL += $"&keyword={KeyWord}";
                }
                viewModel.ApiUrl = URL;
            });
            Init();
        }
        public async void Init()
        {
            DataListView.ItemTapped += DataListView_ItemTapped;
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

        private async void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            await Navigation.PushAsync(new PostDetailPage(post.Id));
        }
        public void Search_Pressed(object sender, EventArgs e)
        {
            KeyWord = searchBar.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                KeyWord = null;
                viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
