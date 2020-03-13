using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using SundihomeApp.Views.MoiGioiViews;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class ThongTinMoiGioiPage : ContentPage
    {
        public ThongTinMoiGioiPageViewModel viewModel;
        public DangKyMoiGioiContentView dangKyMoiGioiView;
        private Guid _moiGioiId;
        private bool _isMoiGioi;
        private int CurrentIndex = 0;

        public ThongTinMoiGioiPage(Guid moiGioiId, bool isMoiGioi)
        {
            InitializeComponent();
            BindingContext = viewModel = new ThongTinMoiGioiPageViewModel(moiGioiId);
            _moiGioiId = moiGioiId;
            _isMoiGioi = isMoiGioi;

            Init();
        }

        public async void Init()
        {
            viewModel.IsMoiGioi = _isMoiGioi;
            SetFloatingButtonGroup();
            await Task.WhenAll(viewModel.LoadThongTinMoiGioi(_moiGioiId),
                viewModel.GetFollowers(_moiGioiId),
                viewModel.LoadData());

            BDSList.ItemTemplate = new DataTemplate(typeof(Cells.PostViewCell));
            BDSList.ItemTapped += Lv_ItemTapped;

            loadingPopup.IsVisible = false;
        }
        private void SetFloatingButtonGroup()
        {
            if (_isMoiGioi)
            {
                btnEdit.IsVisible = true;
                floatingButtonGroup.IsVisible = false;
            }
            else
            {
                if (viewModel.ButtonCommandList.Count < 2)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, Call_Clicked));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem("Chat", FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, Chat_Clicked));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessage_Clicked));
                }
            }
        }

        private void SegmentSelected_Tapped(object sender, EventArgs e)
        {
            RadBorder radBorderSelected = sender as RadBorder;
            TapGestureRecognizer tap = radBorderSelected.GestureRecognizers[0] as TapGestureRecognizer;
            CurrentIndex = int.Parse(tap.CommandParameter.ToString());

            int InactiveIndex = CurrentIndex == 1 ? 0 : 1;

            radBorderSelected.BackgroundColor = (Color)App.Current.Resources["MainDarkColor"];
            (radBorderSelected.Content as Label).TextColor = Color.White;

            var radBorder = (Segment.Children[InactiveIndex] as RadBorder);
            radBorder.BackgroundColor = Color.White;
            (radBorder.Content as Label).TextColor = Color.FromHex("#333333");

            if (CurrentIndex == 0)
            {
                Information.IsVisible = true;
                BDSList.IsVisible = false;
            }
            else
            {
                Information.IsVisible = false;
                BDSList.IsVisible = true;
            }
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            Shell.Current.Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        void OnFollowTapped(object sender, EventArgs e)
        {
            viewModel.Follow(_moiGioiId);
        }

        async void OnUnFollowTapped(object sender, EventArgs e)
        {
            var result = await DisplayAlert(Language.thong_bao, Language.ban_chac_chan_muon_huy_theo_doi, Language.dong_y, Language.huy);
            if (result)
                viewModel.Follow(_moiGioiId);
        }

        public async void Call_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            if (UserLogged.Id == viewModel.MoiGioi.Id.ToString())
            {
                return;
            }
            try
            {
                PhoneDialer.Open(viewModel.MoiGioi.User.Phone);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        public async void Chat_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new ChatPage(viewModel.MoiGioi.Id.ToString()));
        }

        public async void SendMessage_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            if (UserLogged.Id == viewModel.MoiGioi.Id.ToString())
            {
                return;
            }
            try
            {
                await Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.MoiGioi.User.Phone));
            }
            catch
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        private async void GotoProfile_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(viewModel.MoiGioi.User));
        }

        private async void OnEditInfo_Tapped(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (ModalDangKyMoiGioi.Body == null)
            {
                dangKyMoiGioiView = new DangKyMoiGioiContentView(LookUpModal, Guid.Parse(UserLogged.Id));
                dangKyMoiGioiView.OnSaved += async (s, e2) =>
                {
                    dangKyMoiGioiView.viewModel.MoiGioi.UserId = viewModel.MoiGioi.UserId;
                    dangKyMoiGioiView.viewModel.MoiGioi.User = viewModel.MoiGioi.User;
                    dangKyMoiGioiView.viewModel.MoiGioi.RegisterDate = viewModel.MoiGioi.RegisterDate;
                    viewModel.MoiGioi = dangKyMoiGioiView.viewModel.MoiGioi;
                    viewModel.GetTypeFormatString(viewModel.MoiGioi.Type);
                    await ModalDangKyMoiGioi.Hide();
                };
                dangKyMoiGioiView.OnCancel += async (object s, EventArgs e2) => await ModalDangKyMoiGioi.Hide();
                ModalDangKyMoiGioi.Body = dangKyMoiGioiView;
                ModalDangKyMoiGioi.CustomCloseButton(dangKyMoiGioiView.Cancel_Clicked);
            }
            dangKyMoiGioiView.InitUpdate(viewModel.MoiGioi);
            await ModalDangKyMoiGioi.Show();
            loadingPopup.IsVisible = false;
        }

        private async void OnChooseImage_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(Language.chon_anh_chung_chi, Language.huy, null, Language.thu_vien, Language.chup_hinh);
            if (action == Language.chup_hinh)
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (cameraStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await DisplayAlert(Language.quyen_truy_cap_may_anh, Language.sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_anh_chung_chi, Language.tiep_tuc);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        cameraStatus = await PermissionHelper.CheckPermissions(Permission.Camera, Language.quyen_truy_cap_may_anh, Language.sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_anh_chung_chi);
                    }
                    else
                    {
                        var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        if (response.ContainsKey(Permission.Camera))
                            cameraStatus = response[Permission.Camera];
                    }
                }
                if (cameraStatus == PermissionStatus.Granted)
                {
                    await TakePhoto();
                }
            }
            if (action == Language.thu_vien)
            {
                var photoStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                if (photoStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                        await DisplayAlert(Language.quyen_truy_cap_thu_vien, Language.sundihome_can_truy_cap_vao_may_anh_de_chon_hinh_anh_chung_chi, Language.tiep_tuc);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        photoStatus = await PermissionHelper.CheckPermissions(Permission.Photos, Language.quyen_truy_cap_thu_vien, Language.sundihome_can_truy_cap_vao_may_anh_de_chon_hinh_anh_chung_chi);
                    }
                    else
                    {
                        var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                        if (response.ContainsKey(Permission.Photos))
                        {
                            photoStatus = response[Permission.Photos];
                        }
                    }
                }
                if (photoStatus == PermissionStatus.Granted)
                {
                    await PickPhoto();
                }
            }
        }

        // choose picture from library
        async Task PickPhoto()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert(Language.quyen_truy_cap_bi_tu_choi, Language.khong_the_truy_cap_vao_thu_vien_anh, Language.dong);
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            //UpdateAvatar(file, fileName);
        }

        // take new photo from camera
        async Task TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert(Language.thong_bao, Language.may_anh_khong_kha_dung, Language.dong);
                return;
            }

            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Name = fileName,
                SaveToAlbum = false
            });
            //UpdateAvatar(file, fileName);
        }

        private async void QuanLyMoiGioi_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//QuanLyMoiGioi");
        }
    }
}
