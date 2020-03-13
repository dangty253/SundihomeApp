using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class PostItemListPage : ContentPage
    {
        public PostItemListPageViewModel viewModel;
        public IPostItemService postItemService;
        public PostItemListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new PostItemListPageViewModel();
            postItemService = DependencyService.Get<IPostItemService>();
            Init();
        }

        private async void LV_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as PostItem;
            await Navigation.PushAsync(new PostItemDetailPage(item));
        }

        public async void Init()
        {
            LV.ItemTapped += LV_ItemTapped;
            await viewModel.LoadData();
            SubscribeEvent();
            SetColorFilter();

            // khi xoa item tu post detail page.
            MessagingCenter.Subscribe<PostItemDetailPage, string>(this, "OnDeleteSuccess", (o, id) =>
             {
                 var item = this.viewModel.Data.SingleOrDefault(x => x.Id == id);
                 if (item != null)
                 {
                     viewModel.Data.Remove(item);
                 }
             });
            MessagingCenter.Subscribe<PostItemDetailPage, PostItem>(this, "OnUpdateFollow", (o, postItem) =>
            {
                var selectedPost = viewModel.Data.SingleOrDefault(x => x.Id == postItem.Id);
                if (selectedPost == null) return;

                selectedPost.IsFollow = true;
            });

            loadingPopup.IsVisible = false;
        }
        private void SubscribeEvent()
        {
            MessagingCenter.Subscribe<AddPostItemPage, PostItem>(this, "AddPostItemSuccess", async (page, newPostItem) =>
            {
                await viewModel.LoadOnRefreshCommandAsync();
                await Plugin.Share.CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage
                {
                    Url = ApiConfig.WEB_IP + $"postitem/{newPostItem.Id}"
                });
            });
        }
        private void SetColorFilter()
        {
            var stackLayoutFilter = ScrollViewFilter.Content as StackLayout;
            Color MainDarkColor = (Color)App.Current.Resources["MainDarkColor"];
            for (int i = 0; i < stackLayoutFilter.Children.Count; i++)
            {
                var radBorder = stackLayoutFilter.Children[i] as RadBorder;
                var tap = new TapGestureRecognizer()
                {
                    NumberOfTapsRequired = 1,
                    CommandParameter = i
                };
                tap.Tapped += async (sender, e) =>
               {
                   loadingPopup.IsVisible = true;
                   var tappedRadborder = sender as RadBorder;
                   var ortherRadBorder = stackLayoutFilter.Children.Where(x => x != radBorder);

                   foreach (RadBorder item in ortherRadBorder)
                   {
                       item.BackgroundColor = Color.White;
                       (item.Content as Label).TextColor = Color.FromHex("#444444");
                   }

                   var index = stackLayoutFilter.Children.IndexOf(tappedRadborder);
                   tappedRadborder.BackgroundColor = (Color)App.Current.Resources["MainDarkColor"];
                   (tappedRadborder.Content as Label).TextColor = Color.White;

                   viewModel.Type = index - 1;
                   await viewModel.LoadOnRefreshCommandAsync();
                   loadingPopup.IsVisible = false;
               };
                radBorder.GestureRecognizers.Add(tap);
            }
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

        private async void AddNewPost_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (UserLogged.IsLogged == false)
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new AddPostItemPage());
            loadingPopup.IsVisible = false;
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

            PostItem post = (PostItem)(grid.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

            if (post.CreatedById == UserLogged.Id)
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
                return;
            }

            var postId = post.Id;
            bool isFollow = await viewModel.Follow(postId);
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

        public async void Option_Clicked(object sender, EventArgs e)
        {
            int VIEW_DETAIL = 0;
            int DELETE_POST = 1;
            int VIEW_PROFILE = 2;
            int CHAT = 3;

            var postItem = (sender as Button).CommandParameter as PostItem;
            IDictionary<int, string> keyValues = new Dictionary<int, string>();
            keyValues[VIEW_DETAIL] = Language.xem_chi_tiet;
            keyValues[VIEW_PROFILE] = Language.thong_tin_ca_nhan;

            if (UserLogged.IsLogged && postItem.CreatedBy.UserId == UserLogged.Id)
            {
                keyValues[DELETE_POST] = Language.xoa_bai_dang;
            }
            else
            {
                keyValues[CHAT] = "Chat";
            }

            var result = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, keyValues.Values.ToArray());
            if (result == keyValues[VIEW_DETAIL])
            {
                await Navigation.PushAsync(new PostItemDetailPage(postItem.Id));
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
    }
}
