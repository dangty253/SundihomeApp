using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        private User _user;
        public ProfilePageViewModel viewModel;

        static double updateRate = 1000 / 15f; // 30Hz
        static double step = updateRate / (2 * 15 * 1000f);

        public ProfilePage(User user)
        {
            InitializeComponent();
            _user = user;
            BindingContext = viewModel = new ProfilePageViewModel(_user);
            ChangePhonePopup.Body.BindingContext = viewModel;
            Init();

            ChangePasswordPopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
            ChangeEmailPopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
            ChangePhonePopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
            OtpPopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);

            MessagingCenter.Subscribe<ProfilePageViewModel, bool>(this, "OtpPopup", async (sender, arg) =>
            {
                OtpPopup.IsVisible = arg;
                entryOTP1.Focus();
                progressBar.Progress = 1;
                spReset.TextColor = Color.Gray;
                Device.StartTimer(TimeSpan.FromMilliseconds(updateRate), () =>
                {
                    if (progressBar.Progress > 0)
                    {
                        Device.BeginInvokeOnMainThread(() => progressBar.Progress -= step);
                        return true;
                    }
                    lblResetOtp.IsEnabled = true;
                    spReset.TextColor = Color.FromHex("0089D1");
                    return false;
                });
            });

            MessagingCenter.Subscribe<ProfilePageViewModel, User>(this, "UpdateProfile", async (sender, arg) =>
            {
                viewModel.User = arg;
                image.Source = arg.AvatarFullUrl;
            });

            MessagingCenter.Subscribe<ProfilePageViewModel, bool>(this, "ClosePopup", async (sender, arg) =>
            {
                ChangePasswordPopup.IsVisible = ChangePhonePopup.IsVisible = ChangeEmailPopup.IsVisible = arg;
            });

            ChangePasswordPopup.CustomCloseButton(OnCloseChangePassword);
            ChangeEmailPopup.CustomCloseButton(OnCloseChangeEmail);
            ChangePhonePopup.CustomCloseButton(OnCloseChangePhone);
        }

        // change avatar
        async void OnEditAvatarClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(Language.chon_hinh_dai_dien, Language.huy, null, Language.thu_vien_anh, Language.chup_hinh);
            if (action == Language.chup_hinh)
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (cameraStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await DisplayAlert(Language.quyen_truy_cap_may_anh, Language.sundihome_can_quyen_truy_cap_vao_may_anh_anh_dai_dien, Language.dong);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        cameraStatus = await PermissionHelper.CheckPermissions(Permission.Camera, Language.quyen_truy_cap_may_anh, Language.sundihome_can_quyen_truy_cap_vao_may_anh_anh_dai_dien);
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
            if (action == Language.thu_vien_anh)
            {
                var photoStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                if (photoStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                        await DisplayAlert(Language.quyen_truy_cap_thu_vien, Language.sundihome_can_quyen_truy_cap_vao_thu_vien_anh_dai_dien, Language.dong);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        photoStatus = await PermissionHelper.CheckPermissions(Permission.Photos, Language.quyen_truy_cap_thu_vien, Language.sundihome_can_quyen_truy_cap_vao_thu_vien_anh_dai_dien);
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
            UpdateAvatar(file, fileName);
        }

        // take new photo from camera
        async Task TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("", Language.may_anh_khong_kha_dung, Language.dong);
                return;
            }

            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Name = fileName,
                SaveToAlbum = false
            });
            UpdateAvatar(file, fileName);
        }

        // update avatar to sql server and mongodb.
        async void UpdateAvatar(Plugin.Media.Abstractions.MediaFile file, string fileName)
        {
            if (file == null)
            {
                return;
            }
            viewModel.IsLoading = true;
            StreamContent content = new StreamContent(file.GetStream());
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "files",
                FileName = fileName
            };

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(content);

            var apiResponse = await BsdHttpClient.Instance().PostAsync(ApiRouter.USER_AVATAR_UPLOAD, form);
            if (apiResponse.IsSuccessStatusCode)
            {
                string AvatarUrl = "avatar/" + fileName;
                ApiResponse response = await ApiHelper.Post(ApiRouter.USER_AVATAR_UPDATE, AvatarUrl, true, false);
                if (response.IsSuccess)
                {
                    viewModel.User.AvatarUrl = AvatarUrl;

                    string AvatarFullUrl = viewModel.User.AvatarFullUrl;

                    image.Source = AvatarFullUrl;
                    UserLogged.SaveAvatar(AvatarFullUrl);

                    MessagingCenter.Send<ProfilePage, string>(this, "UpdateAvatar", AvatarUrl);
                    await DisplayAlert("", Language.cap_nhat_anh_dai_dien_thanh_cong, Language.dong);
                }
                else
                {
                    await DisplayAlert("", Language.khong_cap_nhat_duoc_anh_dai_dien, Language.dong);
                }
            }
            else
            {
                await DisplayAlert("", Language.khong_cap_nhat_duoc_anh_dai_dien, Language.dong);
            }
            viewModel.IsLoading = false;
        }

        void Gender_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.GenderSelected != null)
            {
                viewModel.User.Sex = (short)viewModel.GenderSelected.Id;
            }
            else
            {
                viewModel.User.Sex = -1;
            }
        }

        //edit phone
        private async void OnEditPhoneClicked(object sender, EventArgs e)
        {
            await ChangePhonePopup.Show();
        }
        private async void OnCloseChangePhone(object sender, EventArgs e)
        {
            await ChangePhonePopup.Hide();
            entryPhone.Text = null;
        }
        private async void OnChangePhone_Clicked(object sender, EventArgs e)
        {
            viewModel.NewPhone = entryPhone.Text;
            await viewModel.OnChangePhone();
        }

        //edit email
        private async void OnEditEmailClicked(object sender, EventArgs e)
        {
            await ChangeEmailPopup.Show();
        }
        private async void OnCloseChangeEmail(object sender, EventArgs e)
        {
            await ChangeEmailPopup.Hide();
            entryEmail.Text = null;
        }
        private async void OnChangeEmail_Clicked(object sender, EventArgs e)
        {
            viewModel.NewEmail = entryEmail.Text;
            await viewModel.OnChangeEmail();
        }

        //edit password
        private async void OnEditPasswordClicked(object sender, EventArgs e)
        {
            await ChangePasswordPopup.Show();
        }
        private async void OnCloseChangePassword(object sender, EventArgs e)
        {
            await ChangePasswordPopup.Hide();
            entryPassword.Text = null;
            entryNewPassword.Text = null;
            entryConfirm.Text = null;
        }
        private async void OnChangePassword_Clicked(object sender, EventArgs e)
        {
            viewModel.ChangePassword = new ChangePasswordModel
            {
                Password = entryPassword.Text,
                NewPassword = entryNewPassword.Text,
                Confirm = entryConfirm.Text
            };
            await viewModel.OnChangePassword();
        }

        //choose birthdate
        void OnOpenDatePicker(object sender, EventArgs e)
        {
            BirthDateDatePicker.Focus();
            viewModel.IsNullBirthday = false;
            viewModel.IsBirthdayHasValue = true;
            CleanBirthDate.IsVisible = true;
            viewModel.IsChangeProfile = true;
        }

        void OnCleanBirthDate(object sender, EventArgs e)
        {
            viewModel.IsNullBirthday = true;
            viewModel.IsBirthdayHasValue = false;
            CleanBirthDate.IsVisible = false;
            viewModel.User.Birthday = null;
            viewModel.IsChangeProfile = true;
        }

        //back to account page
        void OnBackButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        //OTP
        void OnClosePopup(object sender, EventArgs e)
        {
            OtpPopup.IsVisible = false;
            progressBar.Progress = 0;
            OnCleanOTPButtonClicked(sender, e);
        }

        void EntryOTP1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(entryOTP1.Text) && entryOTP1.Text.Length == entryOTP1.MaxLength)
            {
                entryOTP2.Focus();
            }
        }

        void EntryOTP2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(entryOTP2.Text) && entryOTP2.Text.Length == entryOTP2.MaxLength)
            {
                entryOTP3.Focus();
            }
        }

        void EntryOTP3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(entryOTP3.Text) && entryOTP3.Text.Length == entryOTP3.MaxLength)
            {
                entryOTP4.Focus();
            }
        }

        void EntryOTP4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(entryOTP4.Text) && entryOTP4.Text.Length == entryOTP4.MaxLength)
            {
                viewModel.ConfirmOtp();
            }
        }

        public void OnCleanOTPButtonClicked(object sender, EventArgs e)
        {
            entryOTP1.Text = entryOTP2.Text = entryOTP3.Text = entryOTP4.Text = null;
            entryOTP1.Focus();
        }

        public void OnResetOTP(object sender, EventArgs e)
        {
            OnCleanOTPButtonClicked(sender, e);
            viewModel.ResetOTP();
            lblResetOtp.IsEnabled = false;
            progressBar.Progress = 1;
            spReset.TextColor = Color.Gray;

            Device.StartTimer(TimeSpan.FromMilliseconds(updateRate), () =>
            {
                if (progressBar.Progress > 0)
                {
                    Device.BeginInvokeOnMainThread(() => progressBar.Progress -= step);
                    return true;
                }
                lblResetOtp.IsEnabled = true;
                spReset.TextColor = Color.FromHex("0089D1");
                return false;
            });
        }

        //
        public async void Init()
        {
            await viewModel.GetProvinceAsync();
            //get profile
            viewModel.UserProfile.ProvinceId = _user.ProvinceId;
            viewModel.UserProfile.DistrictId = _user.DistrictId;
            viewModel.UserProfile.WardId = _user.WardId;
            viewModel.UserProfile.Street = _user.Street;
            viewModel.UserProfile.Address = _user.Address;
            await Task.WhenAll(viewModel.GetDistrictAsync(),
                viewModel.GetWardAsync());
            viewModel.UserProfile.Province = viewModel.ProvinceList.SingleOrDefault(x => x.Id == viewModel.UserProfile.ProvinceId);
            viewModel.UserProfile.District = viewModel.DistrictList.SingleOrDefault(x => x.Id == viewModel.UserProfile.DistrictId);
            viewModel.UserProfile.Ward = viewModel.WardList.SingleOrDefault(x => x.Id == viewModel.UserProfile.WardId);
        }

        //address
        public async void Province_Change(object sender, LookUpChangeEvent e)
        {

            if (viewModel.UserProfile.Province != null)
            {
                viewModel.UserProfile.ProvinceId = viewModel.UserProfile.Province.Id;
            }
            else
            {
                viewModel.UserProfile.ProvinceId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.UserProfile.District = null;
            viewModel.UserProfile.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.UserProfile.District != null)
            {
                viewModel.UserProfile.DistrictId = viewModel.UserProfile.District.Id;
            }
            else
            {
                viewModel.UserProfile.DistrictId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.UserProfile.Ward = null;

            loadingPopup.IsVisible = false;
        }


        public void OpenPickerMaQuocGia_Tapped(object sender, EventArgs e)
        {
            PickerMaQuocGia.Focus();
        }
    }
}
