using Stormlion.PhotoBrowser;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SundihomeApp.Services;
using SundihomeApi.Entities;
using Xamarin.Essentials;
using SundihomeApp.IServices;
using SundihomeApp.Resources;

namespace SundihomeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostDetailPage : ContentPage
    {
        public PostDetailPageViewModel viewModel;
        public Guid _id;
        public List<Photo> GetPhotos = new List<Photo>();
        private bool IsFollow = false;
        public PostDetailPage(Guid Id)
        {
            _id = Id;
            InitializeComponent();
            this.BindingContext = viewModel = new PostDetailPageViewModel(_id);
            Init();
        }

        public async void Init()
        {
            bool HasResult = await viewModel.LoadPost(_id);
            if (!HasResult)
            {
                await DisplayAlert("", Language.bai_dang_khong_ton_tai_hoac_da_xoa, Language.dong);
                await Navigation.PopAsync();
                return;
            }
            // cau hinh button.
            SetFloatingButtonGroup();


            // can thue can mua thi load comment ve. hien thi o comment.
            if (viewModel.GetPost.PostType == 2 || viewModel.GetPost.PostType == 3)
            {
                await viewModel.LoadPostComment();
                StacklayoutPostComments.IsVisible = true;
                onClick_HideShow_Description(null, EventArgs.Empty); // dong lai mo ta. neu post type 2 3
                MessagingCenter.Subscribe<PickPostPage>(this, "OnNewPostComment", async (sender) =>
                {
                    loadingPopup.IsVisible = true;
                    viewModel.PostComments.Clear();
                    viewModel.PostCommentPage = 1;
                    await viewModel.LoadPostComment();
                    loadingPopup.IsVisible = false;
                });
            }
            else
            {
                StacklayoutPostComments.IsVisible = false;
            }

            lbLoaiBatDongSan.Text = LoaiBatDongSanModel.GetList(null).SingleOrDefault(x => x.Id == viewModel.GetPost.LoaiBatDongSanId).Name;

            if (viewModel.GetPost.PostType == 0 || viewModel.GetPost.PostType == 1)
            {
                stListImg.IsVisible = true;
            }


            if (viewModel.GetPost.PostType == 2 || viewModel.GetPost.PostType == 3)
            {
                gridContent.Margin = new Thickness(0, 10, 0, 0); // đây frame xuong
                cvBDSCungKhuVuc.HeightRequest = 90;
                frContentPost.Margin = new Thickness(10, 0, 10, 0);
            }

            //lấy danh sách Images
            if (viewModel.GetPost.ImageList != null)
            {
                string[] ListImages = viewModel.GetPost.ImageList;
                for (int i = 0; i < ListImages.Length; i++)
                {
                    Photo photo = new Photo();
                    photo.URL = ImageHelper.GetImageUrl("post", ListImages[i]);
                    GetPhotos.Add(photo);
                }
                lbNumImages.Text = ListImages.Count().ToString();
                carouseView.ItemsSource = GetPhotos;
            }

            if (viewModel.GetPost.TinhTrangPhapLyId.HasValue)
            {
                var TinhTrangPhapLy = viewModel.GetPost.TinhTrangPhapLyId;
                List<TinhTrangPhapLyModel> tinhtrangphaplyModel = TinhTrangPhapLyData.GetList();
                TinhTrangPhapLyModel modelTinhTrangPhapLy = tinhtrangphaplyModel.Where(x => x.Id == TinhTrangPhapLy).SingleOrDefault();
                lbTinhTrangPhapLy.Text = modelTinhTrangPhapLy.Name;
            }

            if (viewModel.GetPost.HuongId.HasValue)
            {
                var HuongNha = viewModel.GetPost.HuongId;
                List<HuongModel> huongnhaModel = HuongData.GetList();
                HuongModel modelHuong = huongnhaModel.Where(x => x.Id == HuongNha).SingleOrDefault();
                lbHuongNha.Text = modelHuong.Name;
            }

            //set trang thai duu an.
            if (viewModel.GetPost.Project?.Status != null)
            {
                int id_Status = viewModel.GetPost.Project.Status.Value;
                List<ProjectStatusModel> listStatus = ProjectStatusData.GetList();
                ProjectStatusModel modelStatus = listStatus.Single(x => x.Id == id_Status);
                spStatus.Text = modelStatus.Name;
            }


            //kiểm tra là lấy thông tin của "nội thất tiện nghi"
            if (viewModel.GetPost.Utilities != null)
            {
                string[] utilitiesArr = viewModel.GetPost.Utilities.Split(',');
                Option[] utilitiesImg = new Option[utilitiesArr.Length];
                List<Option> tienichduan = BDSUtilities.GetListUtilities();
                for (int i = 0; i < utilitiesArr.Length; i++)
                {
                    var item = utilitiesArr[i];
                    Option model = tienichduan.Where(x => x.Id == short.Parse(item)).SingleOrDefault();
                    utilitiesImg[i] = model;
                }
                flvUtilities.FlowItemsSource = utilitiesImg;
                stNoiThatTienNghi.IsVisible = true;
            }
            else
            {
                stNoiThatTienNghi.IsVisible = false;
            }

            //kiểm tra. nếu có projectId thì cho hiện "Thuộc dự an" và lấy loại bất động sản
            if (viewModel.GetPost.ProjectId.HasValue)
            {
                //
                stThuocDuAn.IsVisible = true;
                Guid _idProject = viewModel.GetPost.ProjectId.Value;
                await viewModel.LoadBDSKhac(_id, _idProject);
                if (viewModel.BDSKhac.Count != 0)
                {
                    stBDSKhac.IsVisible = true;
                }

                // lấy loại bất động sản
                string[] categoriArr = viewModel.GetPost.Project.CategoriBDS.Split(',');
                string[] newCategoriArr = new string[categoriArr.Length];

                List<ProjectTypeModel> modelCategori = ProjectTypeData.GetListProjectType();
                for (int i = 0; i < categoriArr.Length; i++)
                {
                    var temp = categoriArr[i];
                    ProjectTypeModel categoriModel = modelCategori.Where(x => x.Id == short.Parse(temp)).SingleOrDefault();
                    newCategoriArr[i] = categoriModel.Name;
                }

                lbCatogoriBDS.Text = string.Join(", ", newCategoriArr);
            }

            //gán dữ liệ cho label
            lbTongDienTichSuDung.Text = viewModel.GetPost.AreaFormatText;
            lbDuongVao.Text = viewModel.GetPost.DuongVao.ToString();
            lbMatTien.Text = viewModel.GetPost.MatTien.ToString();
            lbChieuSau.Text = viewModel.GetPost.ChieuSau.ToString();
            lbTang.Text = viewModel.GetPost.Tang.ToString();
            lbSoTang.Text = viewModel.GetPost.NumOfFloor.ToString();
            lbSoThangMay.Text = viewModel.GetPost.SoThangMay.ToString();
            lbSoDuongNgu.Text = viewModel.GetPost.NumOfBedroom.ToString();
            lbSoPhongtam.Text = viewModel.GetPost.NumOfBathroom.ToString();

            // hiển thị thông tin trong phần "tổng quan dự án" (những thông tin luôn hiện)
            stTinhTrangPhapLy.IsVisible = true;
            stHuongNha.IsVisible = true;
            stTongDienTichSuDung.IsVisible = true;

            //hiển thị thông tin trong phần "tổng quan dự án" (kiểm tra theo loại bất động sản)
            var LoaiBatDongSan = viewModel.GetPost.LoaiBatDongSanId;
            if (LoaiBatDongSan == 0)
            {
                stTang.IsVisible = true;
                stSoPhongTam.IsVisible = true;
                stSoDuongNgu.IsVisible = true;
            }

            if (LoaiBatDongSan == 1)
            {
                stTang.IsVisible = true;
                stSoPhongTam.IsVisible = true;
                stSoDuongNgu.IsVisible = true;
            }

            if (LoaiBatDongSan == 2)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoPhongTam.IsVisible = true;
                stSoDuongNgu.IsVisible = true;
            }

            if (LoaiBatDongSan == 3)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoPhongTam.IsVisible = true;
                stSoDuongNgu.IsVisible = true;
            }

            if (LoaiBatDongSan == 4)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoPhongTam.IsVisible = true;
                stSoDuongNgu.IsVisible = true;
            }

            if (LoaiBatDongSan == 5)
            {
                stTang.IsVisible = true;
            }

            if (LoaiBatDongSan == 6)
            {
                stTang.IsVisible = true;
                stSoThangMay.IsVisible = true;
            }

            if (LoaiBatDongSan == 7)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoPhongTam.IsVisible = true;
                stSoDuongNgu.IsVisible = true;
            }

            if (LoaiBatDongSan == 8)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stChieuSau.IsVisible = true;
            }

            if (LoaiBatDongSan == 9)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stChieuSau.IsVisible = true;
            }

            if (LoaiBatDongSan == 10)
            {
                stDuongVao.IsVisible = true;
                stMatTien.IsVisible = true;
                stChieuSau.IsVisible = true;
            }

            var gridChildren = gridContent.Children.Where(x => x.IsVisible == true).ToList();
            for (int i = 0; i < gridChildren.Count(); i++)
            {
                gridContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(9, GridUnitType.Auto) });

                Grid.SetRow(gridChildren[i], i);

            }

            //check follow
            await ChekcIsFollowPost();

            //disable loading
            loadingPopup.IsVisible = false;
            ScrollView_Detail_M_B.IsVisible = true;
        }

        private void SetFloatingButtonGroup()
        {
            bool IsOwner = UserLogged.IsLogged && Guid.Parse(UserLogged.Id) == viewModel.GetPost.UserId; // kiem tra nguoi dang nhap co phai la nguoi tao ra post nay khong.
            if (IsOwner)
            {
                if (viewModel.GetPost.IsCommitment == true)
                {
                    stThoiGianCamKet.IsVisible = true;
                }

                // co cong ty, va chua chia se (= 1 la da duyet, = 2 la gio chung).
                if (string.IsNullOrEmpty(UserLogged.CompanyId) == false && this.viewModel.GetPost.CompanyStatus != 2 && this.viewModel.GetPost.CompanyStatus != 1)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chia_se_cho_cong_ty, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf14d", null, ShareToCompany));
                }
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.thong_tin_chu_so_huu, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf007", null, GoToOwnerPage_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.dang_san, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf044", null, Post_ToPostItem));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chinh_sua, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf044", null, EditPost_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xoa, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2ed", null, DeletePost_Clicked));
            }
            else
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.theo_doi_bai_dang, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf004", null, FollowPost_Clicked));//1
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, CallOptionSelected));//2
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chat, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, ChatOptionSelected));//3
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessageOptionSelected));//4
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.dat_lich_hem, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf53c", null, CreateOrder_Clicked));//5
            }
        }
        private async void GoToOwnerPage_Clicked(object sender, EventArgs e)
        {
            //if (!string.IsNullOrWhiteSpace(viewModel.GetPost.OwnerFullName) && !string.IsNullOrWhiteSpace(viewModel.GetPost.OwnerPhone))
            //{
            //    await DisplayAlert("","chua co thong tin chu so huu",Language.dong);
            //    return;
            //}
            await Shell.Current.Navigation.PushAsync(new OwnerPostPage(_id));
        }
        private async void DeletePost_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.xac_nhan, Language.ban_co_chac_chan_muon_xoa_khong, Language.xoa, Language.huy);
            if (!answer) return;
            loadingPopup.IsVisible = true;
            var deleteReponse = await ApiHelper.Delete(ApiRouter.POST_DELETE + "/" + _id);
            if (deleteReponse.IsSuccess)
            {
                await Navigation.PopAsync();
                MessagingCenter.Send<PostDetailPage, Guid>(this, "OnDeleteSuccess", _id);
            }
            else
            {
                await DisplayAlert("", deleteReponse.Message, Language.dong);
            }
            loadingPopup.IsVisible = false;
        }
        private void EditPost_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new PostPage(_id));
        }

        protected void OnClickedShowGallery(object sender, EventArgs e)
        {
            var img = sender as Image;
            string url = (img.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;
            var image = GetPhotos.Where(x => x.URL == url).SingleOrDefault();
            var index = GetPhotos.IndexOf(image);
            new PhotoBrowser
            {
                Photos = GetPhotos,
                EnableGrid = true,
                StartIndex = index,
            }.Show();
        }

        public async void onClick_GotoProject(object sender, EventArgs e)
        {
            var stactlayout = sender as StackLayout;
            var tap = stactlayout.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;
            await Shell.Current.Navigation.PushAsync(new ProjectDetailPage(id));
        }

        // event click của list " bất động sản cùng khu vực"
        public async void onClick_BDSCKV(object sender, EventArgs e)
        {
            var post = ((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as Post;
            await Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        // xem thong tin ca nhan
        public async void ViewProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserProfilePage(this.viewModel.GetPost.UserId));
        }

        private async void ViewCommentPostUserProfile_Tapped(object sender, EventArgs e)
        {
            Guid userId = Guid.Parse(((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter.ToString());
            await Navigation.PushAsync(new UserProfilePage(userId));
        }

        private async void ViewPostCommentDetail_Tapped(object sender, EventArgs e)
        {
            Guid postId = Guid.Parse(((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter.ToString());
            await Navigation.PushAsync(new PostDetailPage(postId));
        }
        private async void LoadMorePostComment_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            viewModel.PostCommentPage++;
            await viewModel.LoadPostComment();
            loadingPopup.IsVisible = false;
        }
        private async void ViewPostByProject_Clicked(object sender, EventArgs e)
        {
            FilterModel filterModel = new FilterModel();
            filterModel.ProjectId = viewModel.GetPost.ProjectId.Value;
            await Navigation.PushAsync(new SearchResultPage(filterModel) { Title = Language.bat_dong_san_cung_du_an });
        }

        //event của button gọi
        public async void CallOptionSelected(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                RedirectToLoginPage();
                return;
            }
            if (UserLogged.Id == viewModel.GetPost.UserId.ToString())
            {
                return;
            }
            try
            {
                PhoneDialer.Open(viewModel.GetPost.User.Phone);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        //event của button nhắn tin
        public async void ChatOptionSelected(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                RedirectToLoginPage();
                return;
            }
            await Navigation.PushAsync(new ChatPage(viewModel.GetPost.UserId.ToString()));
        }

        // cho gui tin nhan
        public async void SendMessageOptionSelected(object sender, EventArgs e)
        {

            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                RedirectToLoginPage();
                return;
            }
            try
            {
                await Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.GetPost.User.Phone));
            }
            catch
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        //các button ẩn hiện
        public async void onClick_HideShow_Description(object sender, EventArgs e)
        {
            if (lbAbout.IsVisible == true)
            {
                lbAbout.IsVisible = false;
                await showmore_Description.RelRotateTo(180);
            }
            else
            {
                lbAbout.IsVisible = true;
                await showmore_Description.RelRotateTo(-180);
            }
        }

        public async void onClick_HideShow_TQ(object sender, EventArgs e)
        {
            if (HideShow_TQ.IsVisible == false)
            {
                HideShow_TQ.IsVisible = true;
                await showmore_TQ.RelRotateTo(180);
            }
            else
            {
                HideShow_TQ.IsVisible = false;
                await showmore_TQ.RelRotateTo(-180);
            }
        }

        public async void onClick_HideShow_Utility(object sender, EventArgs e)
        {
            if (HideShow_Utility.IsVisible == false)
            {
                HideShow_Utility.IsVisible = true;
                await showmore_Utility.RelRotateTo(180);
            }
            else
            {
                HideShow_Utility.IsVisible = false;
                await showmore_Utility.RelRotateTo(-180);
            }

        }
        public async void onClick_HideShow_ThoiGianCamKet(object sender, EventArgs e)
        {
            if (stContent_ThoiGianCamKet.IsVisible == false)
            {
                stContent_ThoiGianCamKet.IsVisible = true;
                await showmore_ThoiGianCamKet.RelRotateTo(180);
            }
            else
            {
                stContent_ThoiGianCamKet.IsVisible = false;
                await showmore_ThoiGianCamKet.RelRotateTo(-180);
            }
        }
        public async void onClick_HideShow_TDA(object sender, EventArgs e)
        {

            if (HideShow_TDA.IsVisible == false)
            {
                HideShow_TDA.IsVisible = true;
                await showmore_TDA.RelRotateTo(180);
            }
            else
            {
                HideShow_TDA.IsVisible = false;
                await showmore_TDA.RelRotateTo(-180);
            }
        }

        private async void onClick_HideShow_BatDongSanCungKhuVuc(object sender, EventArgs e)
        {
            if (cvBDSCungKhuVuc.IsVisible == false)
            {
                cvBDSCungKhuVuc.IsVisible = true;
                await showmore_BatDongSanCungDuAn.RelRotateTo(180);
            }
            else
            {
                cvBDSCungKhuVuc.IsVisible = false;
                await showmore_BatDongSanCungDuAn.RelRotateTo(-180);
            }
        }
        public async Task ChekcIsFollowPost()
        {
            if (UserLogged.IsLogged)
            {
                var response = await ApiHelper.Get<object>("api/post/IsFollowPost/" + viewModel.GetPost.Id, true);
                if (response.IsSuccess)
                {
                    this.IsFollow = (bool)response.Content;
                    if (this.IsFollow) // dang theo doi.
                    {
                        //BtnFollowPost.TextColor = Color.Red;
                        this.viewModel.ButtonCommandList[1].Text = Language.bo_theo_doi_bai_dang;
                        this.viewModel.ButtonCommandList[1].FontFamily = FontAwesomeHelper.GetFont("FontAwesomeSolid");
                    }
                    else
                    {
                        //BtnFollowPost.TextColor = Color.Gray;
                    }
                }
            }
        }

        // bam nut theo doi
        public async void FollowPost_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                RedirectToLoginPage();
                return;
            }
            loadingPopup.IsVisible = true;
            if (!this.IsFollow)
            {
                var response = await ApiHelper.Post("api/post/AddToFavoritePosts/" + this.viewModel.GetPost.Id, null, true);
                if (response.IsSuccess)
                {
                    this.IsFollow = true;

                    INotificationService notificationService = DependencyService.Get<INotificationService>();
                    var notification = new NotificationModel()
                    {
                        UserId = viewModel.GetPost.UserId,
                        Title = UserLogged.FullName + Language.dang_theo_doi_tin_dang_cua_ban,
                        NotificationType = NotificationType.ViewPost,
                        PostId = viewModel.GetPost.Id,
                        CreatedDate = DateTime.Now,
                        IsRead = false,
                        Thumbnail = viewModel.GetPost.AvatarFullUrl
                    };

                    await notificationService.AddNotification(notification, Language.theo_doi);
                }
            }
            else
            {
                var response = await ApiHelper.Post("api/post/RemoveFromFavoritePosts/" + this.viewModel.GetPost.Id, null, true);
                if (response.IsSuccess)
                {
                    this.IsFollow = false;
                }
            }

            if (this.IsFollow) // dang theo doi.
            {
                //BtnFollowPost.TextColor = Color.Red;
                this.viewModel.ButtonCommandList[1].Text = Language.bo_theo_doi_bai_dang;
                this.viewModel.ButtonCommandList[1].FontFamily = FontAwesomeHelper.GetFont("FontAwesomeSolid");
            }
            else
            {
                //BtnFollowPost.TextColor = Color.Red;
                this.viewModel.ButtonCommandList[1].Text = Language.theo_doi_bai_dang;
                this.viewModel.ButtonCommandList[1].FontFamily = FontAwesomeHelper.GetFont("FontAwesomeRegular");
            }

            loadingPopup.IsVisible = false;
        }

        // bam nut dua vao lich hen
        public async void CreateOrder_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                RedirectToLoginPage();
                return;
            }
            await Navigation.PushAsync(new AppointmentPage(viewModel.GetPost));
        }

        private async void PickerPost_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new PickPostPage(viewModel.GetPost.Id));
        }

        private async void ShareToCompany(object sender, EventArgs e)
        {
            var response = await ApiHelper.Post(ApiRouter.MOIGIOI_SHAREPOST + "/" + _id, null, true);
            if (response.IsSuccess)
            {
                ToastMessageHelper.ShortMessage(Language.chia_se_thanh_cong);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Message))
                {
                    if (response.Message.ToString() == "tin_nay_da_duoc_share_roi")
                    {
                        await DisplayAlert("", Language.tin_nay_da_duoc_share_roi, Language.dong);
                    }
                    else
                    {
                        await DisplayAlert("", "Cannot share this post", Language.dong);
                    }
                }
                else
                {
                    await DisplayAlert("", "Cannot share this post", Language.dong);
                }
            }
        }

        private async void Post_ToPostItem(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            var apiResponse = await ApiHelper.Get<Post>($"api/post/{this._id}");
            var post = apiResponse.Content as Post;
            await Navigation.PushAsync(new AddPostItemPage(post));
            loadingPopup.IsVisible = false;
        }

        private void RedirectToLoginPage()
        {
            ((AppShell)Shell.Current).SetLoginPageActive();
        }
    }
}