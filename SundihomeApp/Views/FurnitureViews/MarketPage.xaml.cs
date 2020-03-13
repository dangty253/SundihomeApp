using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class MarketPage : ContentPage
    {
        public MarketPageViewModel viewModel;
        public IFurniturePostItemService postItemService;
        public MarketPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new MarketPageViewModel();
            LV.ItemTapped += LV_ItemTapped;
            postItemService = DependencyService.Get<IFurniturePostItemService>();
            Init();
        }

        private async void LV_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as FurniturePostItem;
            await Navigation.PushAsync(new FurniturePostItemDetailPage(item));
        }

        public async void Init()
        {
            await viewModel.LoadData();
            SubscribeEvent();
            SetColorFilter();


            // khi xoa item tu post detail page.
            MessagingCenter.Subscribe<FurniturePostItemDetailPage, string>(this, "OnDeleteSuccess", (o, id) =>
            {
                var item = this.viewModel.Data.SingleOrDefault(x => x.Id == id);
                if (item != null && (viewModel.Type == -1 || viewModel.Type == item.Type))
                {
                    viewModel.Data.Remove(item);
                }
            });
            MessagingCenter.Subscribe<FurniturePostItemDetailPage, FurniturePostItem>(this, "OnUpdateFollow", (o, postItem) =>
            {
                var selectedPost = viewModel.Data.SingleOrDefault(x => x.Id == postItem.Id);
                if (selectedPost == null) return;

                selectedPost.IsFollow = true;
            });

            loadingPopup.IsVisible = false;
        }
        private void SubscribeEvent()
        {
            MessagingCenter.Subscribe<AddFurniturePostItemPage, FurniturePostItem>(this, "AddPostItemSuccess", (page, newPostItem) =>
            {
                // cung type moi insert
                if (newPostItem.Type == viewModel.Type || viewModel.Type == -1)
                {
                    viewModel.Data.Insert(0, newPostItem);
                }
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
                    var tappedRadborder = sender as RadBorder;
                    var ortherRadBorder = stackLayoutFilter.Children.Where(x => x != radBorder);

                    foreach (RadBorder item in ortherRadBorder)
                    {
                        item.BackgroundColor = Color.White;
                        (item.Content as Label).TextColor = Color.FromHex("#444444");
                    }

                    var index = stackLayoutFilter.Children.IndexOf(tappedRadborder);
                    tappedRadborder.BackgroundColor = MainDarkColor;
                    (tappedRadborder.Content as Label).TextColor = Color.White;

                    viewModel.Type = index - 1;
                    viewModel.RefreshCommand.Execute(null);
                };
                radBorder.GestureRecognizers.Add(tap);
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
            await Navigation.PushAsync(new AddFurniturePostItemPage());
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

            FurniturePostItem post = (FurniturePostItem)(grid.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

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

        public async void Option_Clicked(object sender, EventArgs e)
        {
            int VIEW_DETAIL = 0;
            int DELETE_POST = 1;
            int VIEW_PROFILE = 2;
            int CHAT = 3;

            var postItem = (sender as Button).CommandParameter as FurniturePostItem;
            IDictionary<int, string> keyValues = new Dictionary<int, string>();
            keyValues[VIEW_DETAIL] = Language.xem_chi_tiet;
            keyValues[VIEW_PROFILE] = Language.thong_tin_ca_nhan;

            if (UserLogged.IsLogged && postItem.CreatedBy.UserId == UserLogged.Id)
            {
                keyValues[DELETE_POST] = Language.xoa_bai_dang;
            }
            else
            {
                keyValues[CHAT] = "Sundihome Chat";
            }

            var result = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, keyValues.Values.ToArray());
            if (result == keyValues[VIEW_DETAIL])
            {
                await Navigation.PushAsync(new FurniturePostItemDetailPage(postItem.Id));
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
