using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class B2BDetailPage : ContentPage
    {
        private B2BDetailPageViewModel viewModel;
        private ListViewPageViewModel2<SundihomeApi.Entities.Post> searchPageResultViewModel;

        public B2BDetailPage(B2BPostItem postItem)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new B2BDetailPageViewModel();
            viewModel.PostItem = postItem;
            Init();
        }
        public B2BDetailPage(string postId)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new B2BDetailPageViewModel();
            viewModel.LoadPostById(postId);
            Init();
        }

        private async void ListView0_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (viewModel.CommentPosts.Count == 5)
            {
                await DisplayAlert("", Language.vui_long_chon_toi_da_5_bsd, Language.dong);
                return;
            }
            var item = e.Item as SundihomeApi.Entities.Post;
            var post = new SundihomeApi.Entities.Mongodb.Post()
            {
                PostId = item.Id.ToString(),
                Title = item.Title,
                PriceText = item.PriceFormatText,
                Avatar = item.Avatar,
                Address = item.Address
            };

            if (!viewModel.CommentPosts.Any(x => x.PostId == post.PostId))
            {
                viewModel.CommentPosts.Add(post);
                viewModel.FireOnChangeClearCommentButton();

                //xoa khoi list view
                searchPageResultViewModel.Data.Remove(item);
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.bai_dang_nay_da_duoc_them_vao_binh_luan_roi);
            }
        }

        public async void Init()
        {
            await viewModel.GetComments();
            if (UserLogged.IsLogged)
            {
                FrameNewComment.IsVisible = true;
            }
            SetVisibleFollowButton();
            loadingPopup.IsVisible = false;
        }

        /// <summary>
        /// set an hien nut theo doi.
        /// duoc goi trong ham init.
        /// </summary>
        public void SetVisibleFollowButton()
        {
            if (UserLogged.IsLogged && viewModel.PostItem.CreatedById == UserLogged.Id)
            {
                gridFollow.IsVisible = false;
                var parent = gridFollow.Parent as Grid; // lay grid cah
                parent.ColumnDefinitions.RemoveAt(0); // xoa column defination dau tien.

                Grid.SetColumn(parent.Children[1], 0); // set column binh luan thanh column dau tien.
                Grid.SetColumn(parent.Children[2], 1); // set column chia se thanhc olumn thu 2.

                parent.Children[1].HorizontalOptions = LayoutOptions.Start;
            }
        }

        private async void ViewUserProfile_Clicked(object sender, EventArgs e)
        {

            var tap = (sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer;
            Guid UserId = Guid.Parse(tap.CommandParameter.ToString());
            await Navigation.PushAsync(new UserProfilePage(UserId));
        }

        private async void ViewUserProfile2_Clicked(object sender, EventArgs e)
        {
            var tap = (sender as Label).GestureRecognizers[0] as TapGestureRecognizer;
            Guid UserId = Guid.Parse(tap.CommandParameter.ToString());
            await Navigation.PushAsync(new UserProfilePage(UserId));
        }


        private async void PickerPost_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (searchPageResultViewModel == null) // chua bat popup lan nao.
            {
                this.ListView0.ItemTapped += ListView0_ItemTapped;
                searchPageResultViewModel = new ListViewPageViewModel2<SundihomeApi.Entities.Post>();
                searchPageResultViewModel.PreLoadData = new Command(() =>
                {
                    searchPageResultViewModel.ApiUrl = ApiRouter.COMPANY_GETPOSTLIST + $"?CompanyId={UserLogged.CompanyId}&page={searchPageResultViewModel.Page}&Keyword={viewModel.PostKeyword}&Status=2";
                });

                this.ListView0.BindingContext = searchPageResultViewModel;
                await searchPageResultViewModel.LoadData();
            }
            else
            {
                viewModel.PostKeyword = null;
                ModalPopupSearchBar.Text = null;
                await searchPageResultViewModel.LoadOnRefreshCommandAsync();
            }
            loadingPopup.IsVisible = false;
            await ModalPickerPost.Show();
        }

        private void RemovePostComment_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var postComment = btn.CommandParameter as SundihomeApi.Entities.Mongodb.Post;
            viewModel.CommentPosts.Remove(postComment);

            // kich hoat ham kiem tra xem co show button clear hay khong.
            viewModel.FireOnChangeClearCommentButton();
        }

        private async void ShowComment_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
            else
            {
                EditorComment.Focus();
            }

        }

        private void CancelCommnet_Clicked(object sender, EventArgs e)
        {
            viewModel.CommentPosts.Clear();
            viewModel.CommentText = null;
            viewModel.FireOnChangeClearCommentButton();
        }

        public void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // huy search va dieu kien saerch hien tai khac "" hoac emtpy thi moi chay lai. 
            if (string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(viewModel.PostKeyword))
            {
                Search_Clicked(sender, EventArgs.Empty);
            }
        }

        private void Search_Clicked(object sender, EventArgs e)
        {
            viewModel.PostKeyword = ModalPopupSearchBar.Text ?? "";
            searchPageResultViewModel.RefreshCommand.Execute(null);
        }

        private async void FollowPost_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            loadingPopup.IsVisible = true;
            Grid grid = (sender as Grid);
            var postId = viewModel.PostItem.Id;
            bool isFollow = await viewModel.Follow(postId);
            viewModel.PostItem.IsFollow = isFollow;
            if (isFollow)
            {
                grid.Children[0].IsVisible = false;
                grid.Children[1].IsVisible = false;
                grid.Children[2].IsVisible = true;
                grid.Children[3].IsVisible = true;
            }
            else
            {
                grid.Children[0].IsVisible = true;
                grid.Children[1].IsVisible = true;
                grid.Children[2].IsVisible = false;
                grid.Children[3].IsVisible = false;
            }

            MessagingCenter.Send<B2BDetailPage, B2BPostItem>(this, "OnUpdateFollow", viewModel.PostItem);

            loadingPopup.IsVisible = false;
        }

        private async void SendComment_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.CommentText) && viewModel.CommentPosts.Any() == false)
            {
                await DisplayAlert("", Language.nhap_binh_luan_hoac_chon_tin_dang_de_gui_binh_luan, Language.dong);
                return;
            }

            loadingPopup.IsVisible = true;

            PostItemComment comment = new PostItemComment();

            if (!string.IsNullOrWhiteSpace(viewModel.CommentText))
            {
                comment.HasText = true;
                comment.Text = viewModel.CommentText.Trim();
            }

            comment.PostItemId = viewModel.PostItem.Id;
            comment.CreatedDate = DateTime.Now;
            comment.CreatedById = UserLogged.Id;
            comment.CreatedBy = new PostItemUser() // them cho nay de giao dien co. ko luu db.
            {
                UserId = UserLogged.Id,
                FullName = UserLogged.FullName,
                Avatar = UserLogged.AvatarUrl // neu la lnk full thi avatar full url tra ve nguyen ven.
            };
            if (viewModel.CommentPosts.Any())
            {
                comment.HasPost = true;
                comment.Posts = viewModel.CommentPosts.ToList();
            }

            viewModel.InsertComment(comment);
            viewModel.Comments.Insert(0, comment);


            CancelCommnet_Clicked(sender, EventArgs.Empty);
            EditorComment.Unfocus();

            loadingPopup.IsVisible = false;


            try
            {
                INotificationService notificationService = DependencyService.Get<INotificationService>();
                var AllUserReceiveNoti = await viewModel._postItemService.GetReceiveNotificationUser(viewModel.PostItem.Id);
                //var allUserFollow = 
                string[] CommentUserIds = AllUserReceiveNoti.Where(x => x != UserLogged.Id).ToArray();

                string NotificationTitle = UserLogged.FullName + " "+Language.da_binh_luan_trong_bai_viet;
                string NotificationImage = (viewModel.PostItem.Images != null && viewModel.PostItem.Images.Length > 0) ? AvatarHelper.GetPostAvatar(viewModel.PostItem.Images.FirstOrDefault()) : "";
                foreach (var userIdComment in CommentUserIds)
                {
                    Guid ReceiverId = Guid.Parse(userIdComment);
                    NotificationModel notification = new NotificationModel()
                    {
                        UserId = ReceiverId,
                        CurrentBadgeCount = (int)notificationService.CountNotReadNotificationUser(ReceiverId) + 1,
                        Title = NotificationTitle,
                        NotificationType = NotificationType.ViewB2BPostItem,
                        PostItemId = viewModel.PostItem.Id,
                        CreatedDate = DateTime.Now,
                        IsRead = false,
                        Thumbnail = NotificationImage
                    };
                    await notificationService.AddNotification(notification, Language.binh_luan);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("", Language.co_loi_khi_gui_thong_bao, Language.dong);
            }

        }


        private async void ViewPostDetail_Clicked(object sender, EventArgs e)
        {
            var post = ((sender as ExtendedFrame).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as SundihomeApi.Entities.Mongodb.Post;
            await Navigation.PushAsync(new PostDetailPage(Guid.Parse(post.PostId)));
        }

        public async void Option_Clicked(object sender, EventArgs e)
        {

            int DELETE_POST = 1;
            int VIEW_PROFILE = 2;
            int CHAT = 3;
            int VIEW_COMPANYPROFILE = 4;

            var postItem = viewModel.PostItem;
            IDictionary<int, string> keyValues = new Dictionary<int, string>();
            keyValues[VIEW_PROFILE] = Language.thong_tin_ca_nhan;
            keyValues[VIEW_COMPANYPROFILE] = Language.thong_tin_cong_ty;
            if (UserLogged.IsLogged && postItem.CreatedBy.UserId == UserLogged.Id)
            {
                keyValues[DELETE_POST] = Language.xoa_bai_dang;
            }
            else
            {
                keyValues[CHAT] = Language.sundihome_chat;
            }

            var result = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, keyValues.Values.ToArray());
            if (keyValues.ContainsKey(VIEW_COMPANYPROFILE) && result == keyValues[VIEW_COMPANYPROFILE])
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

                IB2BPostItemService postItemService = DependencyService.Get<IB2BPostItemService>();
                await postItemService.RemovePostItem(postItem.Id);
                await Navigation.PopAsync();

                MessagingCenter.Send<B2BDetailPage, string>(this, "OnDeleteSuccess", postItem.Id);

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
