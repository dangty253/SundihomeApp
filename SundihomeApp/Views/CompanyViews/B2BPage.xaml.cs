using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class B2BPage : ContentPage
    {
        public B2BPageViewModel viewModel;
        public IB2BPostItemService postItemService;
        public B2BPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new B2BPageViewModel();
            LV.ItemTapped += LV_ItemTapped;
            postItemService = DependencyService.Get<IB2BPostItemService>();
            Init();
        }
        private async void Init()
        {
            // khi xoa item tu post detail page.
            MessagingCenter.Subscribe<B2BDetailPage, string>(this, "OnDeleteSuccess", (o, id) =>
            {
                var item = this.viewModel.Data.SingleOrDefault(x => x.Id == id);
                if (item != null)
                {
                    viewModel.Data.Remove(item);
                }
            });
            MessagingCenter.Subscribe<B2BDetailPage, B2BPostItem>(this, "OnUpdateFollow", (o, postItem) =>
            {
                var selectedPost = viewModel.Data.SingleOrDefault(x => x.Id == postItem.Id);
                if (selectedPost == null) return;

                selectedPost.IsFollow = true;
            });

            if (UserLogged.IsLogged && UserLogged.RoleId == 0)
            {
                StackButton.IsVisible = true;
            }

            await viewModel.LoadData();
            SubscribeEvent();
            SetColorFilter();
            loadingPopup.IsVisible = false;
        }


        private async void LV_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as B2BPostItem;
            await Navigation.PushAsync(new B2BDetailPage(item));
        }
        private void SubscribeEvent()
        {
            MessagingCenter.Subscribe<B2BAddPage, B2BPostItem>(this, "AddPostItemSuccess", (page, newPostItem) =>
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
            IDictionary<int, Color> colors = new Dictionary<int, Color>()
            {
                {0,MainDarkColor},
                {1,Color.FromHex(Colors.BAN)},
                {2,Color.FromHex(Colors.CHOTHUE )},

            };
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
                    tappedRadborder.BackgroundColor = colors[index];
                    (tappedRadborder.Content as Label).TextColor = Color.White;

                    viewModel.Type = index - 1;
                    viewModel.RefreshCommand.Execute(null);
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

            B2BPostItem post = (B2BPostItem)(grid.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

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
            int VIEW_COMPANYPROFILE = 4;

            var postItem = (sender as Button).CommandParameter as B2BPostItem;
            IDictionary<int, string> keyValues = new Dictionary<int, string>();
            keyValues[VIEW_DETAIL] = Language.xem_chi_tiet;
            keyValues[VIEW_COMPANYPROFILE] = Language.xem_thong_tin_cong_ty;
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
                await Navigation.PushAsync(new B2BDetailPage(postItem.Id));
            }
            else if (keyValues.ContainsKey(VIEW_COMPANYPROFILE) && result == keyValues[VIEW_COMPANYPROFILE])
            {
                var userResponse = await ApiHelper.Get<User>(ApiRouter.USER_GET_USER_BY_ID + "/" + postItem.CreatedById);
                if (userResponse.IsSuccess)
                {
                    var user = userResponse.Content as User;
                    var companyID = user.CompanyId.Value;
                    await Navigation.PushAsync(new CompanyProfileDetailPage(companyID));
                }
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
            await Navigation.PushAsync(new B2BAddPage());
        }
    }
}
