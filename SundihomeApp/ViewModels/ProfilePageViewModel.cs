using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class ProfilePageViewModel : BaseViewModel
    {
        public List<MaQuocGia> MaQuocGiaList { get; set; } = MaQuocGiaData.GetList();
        private MaQuocGia _maQuocGia;
        public MaQuocGia MaQuocGia { get => _maQuocGia; set { _maQuocGia = value; OnPropertyChanged(nameof(MaQuocGia)); } }

        private UserProfileModel _userProfile;
        public UserProfileModel UserProfile
        {
            get => _userProfile;
            set
            {
                _userProfile = value;
                OnPropertyChanged(nameof(UserProfile));
            }
        }

        public ObservableCollection<Province> ProvinceList { get; set; }
        public ObservableCollection<District> DistrictList { get; set; }
        public ObservableCollection<Ward> WardList { get; set; }

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        private bool _isMale;
        public bool IsMale
        {
            get => _isMale;
            set
            {
                _isMale = value;
                OnPropertyChanged(nameof(IsMale));
            }
        }

        private bool _isFemale;
        public bool IsFemale
        {
            get => _isFemale;
            set
            {
                _isFemale = value;
                OnPropertyChanged(nameof(IsFemale));
            }
        }

        private bool _isOther;
        public bool IsOther
        {
            get => _isOther;
            set
            {
                _isOther = value;
                OnPropertyChanged(nameof(IsOther));
            }
        }

        private bool _isNullBirthday;
        public bool IsNullBirthday
        {
            get => _isNullBirthday;
            set
            {
                _isNullBirthday = value;
                OnPropertyChanged(nameof(IsNullBirthday));
            }
        }

        private bool _isBirthdayHasValue;
        public bool IsBirthdayHasValue
        {
            get => _isBirthdayHasValue;
            set
            {
                _isBirthdayHasValue = value;
                OnPropertyChanged(nameof(IsBirthdayHasValue));
            }
        }

        private bool _isPasswordHasValue;
        public bool IsPasswordHasValue
        {
            get => _isPasswordHasValue;
            set
            {
                _isPasswordHasValue = value;
                OnPropertyChanged(nameof(IsPasswordHasValue));
            }
        }

        private ChangePasswordModel _changePassword;
        public ChangePasswordModel ChangePassword
        {
            get => _changePassword;
            set
            {
                _changePassword = value;
                OnPropertyChanged(nameof(ChangePassword));
            }
        }

        private string _newEmail;
        public string NewEmail
        {
            get => _newEmail;
            set
            {
                _newEmail = value;
                OnPropertyChanged(nameof(NewEmail));
            }
        }

        private string _newPhone;
        public string NewPhone
        {
            get => _newPhone;
            set
            {
                _newPhone = value;
                OnPropertyChanged(nameof(NewPhone));
            }
        }

        private string _otp;

        private OtpModel _registerOtp;
        public OtpModel RegisterOtp
        {
            get => _registerOtp;
            set
            {
                _registerOtp = value;
                OnPropertyChanged(nameof(RegisterOtp));
            }
        }

        private bool _isChangeProfile;
        public bool IsChangeProfile
        {
            get => _isChangeProfile;
            set
            {
                _isChangeProfile = value;
                OnPropertyChanged(nameof(IsChangeProfile));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public List<Option> GenderList { get; set; }

        private Option _genderSelected;
        public Option GenderSelected
        {
            get => _genderSelected;
            set
            {
                _genderSelected = value;
                OnPropertyChanged(nameof(GenderSelected));
            }
        }

        public Command OnUpdateCommand { get; set; }
        public Command OnCancelCommand { get; set; }
        public Command OnConfirmOtpCommand { get; set; }

        public ProfilePageViewModel(User user)
        {
            User = user;
            RegisterOtp = new OtpModel();
            ChangePassword = new ChangePasswordModel();

            GetUserSex();
            GetUserBirthday();

            CheckPasswordHasValue();

            OnUpdateCommand = new Command(OnUpdate);
            OnCancelCommand = new Command(Cancel);
            OnConfirmOtpCommand = new Command(ConfirmOtp);

            UserProfile = new UserProfileModel();
            ProvinceList = new ObservableCollection<Province>();
            DistrictList = new ObservableCollection<District>();
            WardList = new ObservableCollection<Ward>();

            MaQuocGia = this.MaQuocGiaList[0];
        }

        void GetUserSex()
        {
            GenderList = new List<Option>()
            {
                new Option{ Id = 0, Name = Language.nam},
                new Option{ Id = 1, Name = Language.nu},
                new Option{ Id = 2, Name = Language.khac}
            };
            switch (User.Sex)
            {
                case 0:
                    GenderSelected = GenderList[0];
                    break;
                case 1:
                    GenderSelected = GenderList[1];
                    break;
                case 2:
                    GenderSelected = GenderList[2];
                    break;
            }
        }

        void GetUserBirthday()
        {
            if (User.Birthday == DateTime.MinValue || !User.Birthday.HasValue)
            {
                IsNullBirthday = true;
                User.Birthday = null;
            }
            IsBirthdayHasValue = !IsNullBirthday;
        }

        public async void OnUpdate()
        {
            if (string.IsNullOrWhiteSpace(User.FullName))
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_ho_ten, Language.dong);
                return;
            }

            IsLoading = true;
            if (User.Birthday == DateTime.MinValue)
            {
                User.Birthday = null;
            }
            if (User.Sex == -1)
            {
                User.Sex = null;
            }

            //
            if (UserProfile.Province != null)
                User.ProvinceId = UserProfile.Province.Id;
            else
                User.ProvinceId = null;

            if (UserProfile.District != null)
                User.DistrictId = UserProfile.District.Id;
            else
                User.DistrictId = null;

            if (UserProfile.Ward != null)
                User.WardId = UserProfile.Ward.Id;
            else
                User.WardId = null;

            User.Street = UserProfile.Street;
            User.Address = UserProfile.Address;

            ApiResponse apiResponse = await ApiHelper.Put(ApiRouter.USER_PROFILE_UPDATE, User, true);
            if (apiResponse.IsSuccess)
            {
                var profileUpdate = JsonConvert.DeserializeObject<User>(apiResponse.Content.ToString());
                User.FullName = profileUpdate.FullName;
                User.Birthday = profileUpdate.Birthday;
                User.Address = profileUpdate.Address;
                if (!profileUpdate.Sex.HasValue)
                {
                    User.Sex = -1;
                }
                else
                {
                    User.Sex = profileUpdate.Sex;
                }

                if (profileUpdate.AvatarFullUrl != UserLogged.AvatarUrl)
                {
                    User.AvatarUrl = profileUpdate.AvatarUrl;
                    UserLogged.SaveAvatar(User.AvatarFullUrl);
                }

                UserLogged.SaveProfile(User);
                MessagingCenter.Send<ProfilePageViewModel, User>(this, "UpdateProfile", User);
                await Shell.Current.Navigation.PopAsync();
                ToastMessageHelper.ShortMessage(Language.cap_nhat_thong_tin_thanh_cong);
                IsLoading = false;
            }
            else
            {
                await Shell.Current.DisplayAlert("", apiResponse.Message, Language.dong);
            }
            IsLoading = false;
        }

        async void Cancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task OnChangePassword()
        {
            if (ChangePassword == null || string.IsNullOrWhiteSpace(ChangePassword.Password) || string.IsNullOrWhiteSpace(ChangePassword.NewPassword) || string.IsNullOrWhiteSpace(ChangePassword.Confirm))
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_mat_khau, Language.dong);
                return;
            }
            IsLoading = true;
            try
            {
                var response = await ApiHelper.Post("api/user/checkpassword", ChangePassword.Password, true);
                if (!response.IsSuccess)
                {
                    throw new Exception(Language.mat_khau_khong_dung);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("", ex.Message, Language.dong);
                IsLoading = false;
                return;
            }
            if (ChangePassword.NewPassword != ChangePassword.Confirm)
            {
                await Shell.Current.DisplayAlert("", Language.xac_nhan_mat_khau_khong_dung, Language.dong);
                IsLoading = false;
                return;
            }
            if (!Validations.IsValidPassword(ChangePassword.NewPassword))
            {
                await Application.Current.MainPage.DisplayAlert("", Language.mat_khau_so_ky_tu, Language.dong);
                IsLoading = false;
                return;
            }

            try
            {
                var response = await ApiHelper.Put("api/user/changepassword", ChangePassword, true);
                if (response.IsSuccess)
                {
                    UserLogged.SavePassword(ChangePassword.NewPassword);
                    MessagingCenter.Send<ProfilePageViewModel, bool>(this, "ClosePopup", false);
                    ChangePassword = new ChangePasswordModel();
                    await Application.Current.MainPage.DisplayAlert("", Language.cap_nhat_mat_khau_thanh_cong, Language.dong);
                    IsLoading = false;
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                IsLoading = false;
            }
        }

        public async Task OnChangeEmail()
        {
            if (string.IsNullOrWhiteSpace(NewEmail))
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_email, Language.dong);
                return;
            }
            NewEmail = NewEmail.ToLower();
            if (!Validations.IsValidEmail(NewEmail))
            {
                await Shell.Current.DisplayAlert("", Language.email_khong_hop_le, Language.dong);
                return;
            }
            IsLoading = true;
            var response = await ApiHelper.Put(ApiRouter.USER_CHANGEEMAIL, NewEmail, true);
            if (response.IsSuccess)
            {
                User.Email = NewEmail;
                UserLogged.SaveEmail(NewEmail);
                MessagingCenter.Send<ProfilePageViewModel, User>(this, "UpdateProfile", User);
                MessagingCenter.Send<ProfilePageViewModel, bool>(this, "ClosePopup", false);
                NewEmail = null;
                await Shell.Current.DisplayAlert("", Language.cap_nhat_email_thanh_cong, Language.dong);
            }
            else
            {
                if (response.Message != null && response.Message.ToLower().Contains("email đã tồn tại"))
                {
                    await Shell.Current.DisplayAlert("", Language.email_da_ton_tai, Language.dong);
                }
                else
                {
                    await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                }
            }
            IsLoading = false;
        }

        public async Task OnChangePhone()
        {

            try
            {
                if (string.IsNullOrWhiteSpace(NewPhone))
                {
                    throw new Exception(Language.vui_long_nhap_so_dien_thoai);
                }
                if (!Validations.IsValidPhone(NewPhone))
                    throw new Exception(Language.sdt_khong_hop_le);

                IsLoading = true;
                var response = await ApiHelper.Post(ApiRouter.USER_CHECKPHONE, MaQuocGia.Value + NewPhone, true);
                if (response.IsSuccess)
                {
                    try
                    {
                        _otp = StringUtils.RandomString(4);
                        await StringUtils.SendOTP(MaQuocGia.Value + NewPhone, $"{_otp} {Language.la_ma_xac_thuc_cua_ban}");
                        IsLoading = false;
                        MessagingCenter.Send<ProfilePageViewModel, bool>(this, "OtpPopup", true);
                    }
                    catch (Exception ex)
                    {
                        IsLoading = false;
                        await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                    }
                }
                else
                {
                    IsLoading = false;
                    if (response.Message != null && response.Message.ToLower().Contains("số điện thoại đã tồn tại"))
                    {
                        await Shell.Current.DisplayAlert("", Language.sdt_da_ton_tai, Language.dong);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
            }
        }

        public async void ConfirmOtp()
        {
            IsLoading = true;
            if (RegisterOtp.Otp1 + RegisterOtp.Otp2 + RegisterOtp.Otp3 + RegisterOtp.Otp4 != _otp)
            {
                await Application.Current.MainPage.DisplayAlert("", Language.otp_khong_hop_le, Language.dong);
                IsLoading = false;
                return;
            }

            //change
            try
            {
                var response = await ApiHelper.Put("api/user/changephone", (this.MaQuocGia.Value + NewPhone), true);
                if (response.IsSuccess)
                {
                    User.Phone = (this.MaQuocGia.Value + NewPhone);
                    //UserLogged.SavePhone(NewPhone);
                    RegisterOtp = null;
                    MessagingCenter.Send<ProfilePageViewModel, User>(this, "UpdateProfile", User);
                    MessagingCenter.Send<ProfilePageViewModel, bool>(this, "ClosePopup", false);
                    MessagingCenter.Send<ProfilePageViewModel, bool>(this, "OtpPopup", false);
                    NewPhone = null;
                    await Application.Current.MainPage.DisplayAlert("", Language.cap_nhat_sdt_thanh_cong, Language.dong);
                    IsLoading = false;
                }
                else
                {
                    if (response.Message != null && response.Message == "Số điện thoại đã tồn tại!")
                    {
                        throw new Exception(Language.sdt_da_ton_tai);
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                IsLoading = false;
            }
        }

        public async void ResetOTP()
        {
            _otp = StringUtils.RandomString(4);
            try
            {
                await StringUtils.SendOTP(NewPhone, $"{_otp} {Language.la_ma_xac_thuc_cua_ban}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
            }
        }

        async void CheckPasswordHasValue()
        {
            try
            {
                var response = await ApiHelper.Get<object>("api/user/checkpasswordhasvalue", true);
                if (response.IsSuccess)
                {
                    IsPasswordHasValue = (bool)response.Content;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, ex.Message, Language.dong);
            }
        }

        public async Task GetProvinceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            List<Province> data = (List<Province>)apiResponse.Content;
            foreach (var item in data)
            {
                ProvinceList.Add(item);
            }
        }

        public async Task GetDistrictAsync()
        {
            this.DistrictList.Clear();
            if (UserProfile.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{UserProfile.ProvinceId}", false, false);
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    DistrictList.Add(item);
                }
            }
        }

        public async Task GetWardAsync()
        {
            WardList.Clear();
            if (UserProfile.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{UserProfile.DistrictId}", false, false);
                List<Ward> data = (List<Ward>)apiResponse.Content;
                foreach (var item in data)
                {
                    WardList.Add(item);
                }
            }
        }
    }
}
