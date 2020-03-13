using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.IServices;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class InternalPage : ContentPage
    {
        public InternalPageViewModel viewModel;
        public IInternalPostItemService postItemService;
        public InternalPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new InternalPageViewModel();
            postItemService = DependencyService.Get<IInternalPostItemService>();
            Init();
        }
        private async void Init()
        {
            // khi xoa item tu post detail page.
            MessagingCenter.Subscribe<InternalDetailPage, string>(this, "OnDeleteSuccess", (o, id) =>
            {
                var item = this.viewModel.Data.SingleOrDefault(x => x.Id == id);
                if (item != null)
                {
                    viewModel.Data.Remove(item);
                }
            });
            MessagingCenter.Subscribe<InternalDetailPage, InternalPostItem>(this, "OnUpdateFollow", (o, postItem) =>
            {
                var selectedPost = viewModel.Data.SingleOrDefault(x => x.Id == postItem.Id);
                if (selectedPost == null) return;

                selectedPost.IsFollow = true;
            });
            LV.ItemTapped += LV_ItemTapped;
            await viewModel.LoadData();
            SubscribeEvent();
            loadingPopup.IsVisible = false;
        }


        private async void LV_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as InternalPostItem;
            await Navigation.PushAsync(new InternalDetailPage(item));
        }
        private void SubscribeEvent()
        {
            MessagingCenter.Subscribe<InternalAddPage, InternalPostItem>(this, "AddPostItemSuccess", (page, newPostItem) =>
            {
                viewModel.Data.Insert(0, newPostItem);
            });
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            viewModel.Keyword = searchBar.Text;
            loadingPopup.IsVisible = true;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        private void SearchText_Changed(object sender, TextChangedEventArgs e)
        {
            if ((e.NewTextValue == null || e.NewTextValue == "") && !string.IsNullOrWhiteSpace(this.viewModel.Keyword))
            {
                Search_Clicked(null, EventArgs.Empty);
            }
        }
        private async void FollowPost_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (UserLogged.IsLogged == false)
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }

            Grid grid = (sender as Grid);

            InternalPostItem post = (InternalPostItem)(grid.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

            if (post.CreatedById == UserLogged.Id)
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
                return;
            }

            bool isFollow = await viewModel.Follow(post.Id);
            grid.Children[2].IsVisible = isFollow;
            grid.Children[3].IsVisible = isFollow;
            loadingPopup.IsVisible = false;
        }

        private async void ViewUserProfile_Clicked(object sender, EventArgs e)
        {
            var tap = (sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer;
            Guid UserId = Guid.Parse(tap.CommandParameter.ToString());
            await Navigation.PushAsync(new UserProfilePage(UserId));
        }

        private async void ViewPost_Clicked(object sender, EventArgs e)
        {
            var postId = ((sender as ExtendedFrame).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter.ToString();
            await Navigation.PushAsync(new PostDetailPage(Guid.Parse(postId)));
        }

        public async void Option_Clicked(object sender, EventArgs e)
        {
            int VIEW_DETAIL = 0;
            int DELETE_POST = 1;
            int VIEW_PROFILE = 2;
            int CHAT = 3;

            var postItem = (sender as Button).CommandParameter as InternalPostItem;
            IDictionary<int, string> keyValues = new Dictionary<int, string>();
            keyValues[VIEW_DETAIL] = Language.xem_chi_tiet;
            keyValues[VIEW_PROFILE] = Language.thong_tin_ca_nhan;

            if (UserLogged.IsLogged && postItem.CreatedBy.UserId == UserLogged.Id)
            {
                keyValues[DELETE_POST] = Language.xoa_bai_dang;
            }
            else
            {
                keyValues[CHAT] = Language.sundihome_chat;
            }

            var result = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, keyValues.Values.ToArray());
            if (result == keyValues[VIEW_DETAIL])
            {
                await Navigation.PushAsync(new InternalDetailPage(postItem.Id));
            }
            else if (keyValues.ContainsKey(DELETE_POST) && result == keyValues[DELETE_POST])
            {
                var answer = await DisplayAlert(Language.xac_nhan_xoa, Language.ban_co_chac_chan_muon_xoa_tin_dang_nay_khong, Language.xoa, Language.huy);
                if (!answer) return;

                await postItemService.RemovePostItem(postItem.Id);
                viewModel.Data.Remove(postItem);
            }
            else if (result == keyValues[VIEW_PROFILE])
            {
                await Navigation.PushAsync(new UserProfilePage(Guid.Parse(postItem.CreatedBy.UserId)));
            }
            else if (keyValues.ContainsKey(CHAT) && result == keyValues[CHAT])
            {
                if (!UserLogged.IsLogged)
                {
                    await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                    ((AppShell)Shell.Current).SetLoginPageActive();
                    return;
                }
                await Navigation.PushAsync(new ChatPage(postItem.CreatedBy.UserId));
            }
        }

        private async void AddPost_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InternalAddPage());
        }
    }
}

