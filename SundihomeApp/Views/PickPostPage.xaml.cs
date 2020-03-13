using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class PickPostPage : ContentPage
    {
        private Guid _parentPostId;
        private SearchPageResultViewModel viewModel;
        public PickPostPage(Guid ParentPostId)
        {
            InitializeComponent();
            _parentPostId = ParentPostId;

            this.BindingContext = viewModel = new SearchPageResultViewModel();
            viewModel.FilterModel = new FilterModel();
            viewModel.FilterModel.PostType = 0;
            viewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);

            Lv.ItemTemplate = new DataTemplate(typeof(Cells.PostViewCell));
            Init();
        }
        private async void Init()
        {
            Lv.ItemTapped += Item_Tapped;
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void Item_Tapped(object sender, ItemTappedEventArgs e)
        {
            loadingPopup.IsVisible = true;
            var item = e.Item as Post;

            PostComment postComment = new PostComment();
            postComment.ParentPostId = _parentPostId;
            postComment.ChildPostId = item.Id;

            ApiResponse response = await ApiHelper.Post(ApiRouter.POST_COMMENT, postComment, true);
            if (response.IsSuccess)
            {
                loadingPopup.IsVisible = false;
                await Navigation.PopAsync();
                MessagingCenter.Send<PickPostPage>(this, "OnNewPostComment");
                ToastMessageHelper.ShortMessage("Bình luận thành công.");
            }
            else
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", response.Message, Language.dong);
            }
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

        private void Type_Changed(object sender, EventArgs e)
        {
            viewModel.FilterModel.PostType = (short)SegmentedLoaiHinh.SelectedIndex;
            viewModel.RefreshCommand.Execute(null);
        }
    }
}
