using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class ForgetPasswordPageViewModel : BaseViewModel
    {
        public List<MaQuocGia> MaQuocGiaList { get; set; } = MaQuocGiaData.GetList();
        private MaQuocGia _maQuocGia;
        public MaQuocGia MaQuocGia { get => _maQuocGia; set { _maQuocGia = value; OnPropertyChanged(nameof(MaQuocGia)); } }

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

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _confirm;
        public string Confirm
        {
            get => _confirm;
            set
            {
                _confirm = value;
                OnPropertyChanged(nameof(Confirm));
            }
        }

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

        private bool _isChangePasswordVisible;
        public bool IsChangePasswordVisible
        {
            get => _isChangePasswordVisible;
            set
            {
                _isChangePasswordVisible = value;
                OnPropertyChanged(nameof(IsChangePasswordVisible));
            }
        }

        private bool _isPhoneVisible;
        public bool IsPhoneVisible
        {
            get => _isPhoneVisible;
            set
            {
                _isPhoneVisible = value;
                OnPropertyChanged(nameof(IsPhoneVisible));
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

        private HttpClient _client = BsdHttpClient.Instance();
        public Command OnSubmitCommand { get; set; }
        public Command OnConfirmOtpCommand { get; set; }

        public ForgetPasswordPageViewModel()
        {
            RegisterOtp = new OtpModel();
            User = new User();

            IsPhoneVisible = true;

            OnSubmitCommand = new Command(Submit);
            OnConfirmOtpCommand = new Command(ConfirmOtp);

            this.MaQuocGia = this.MaQuocGiaList[0];
        }

        async void Submit()
        {
            IsLoading = true;
            if (IsPhoneVisible)
            {
                // check phone valid
                if (!Validations.IsValidPhone(Phone))
                {
                    await Application.Current.MainPage.DisplayAlert("", Language.sdt_khong_hop_le, Language.dong);
                    IsLoading = false;
                    return;
                }

                try
                {
                    var response = await ApiHelper.Post(ApiRouter.USER_CHECKPHONE, MaQuocGia.Value + Phone);
                    if (response.IsSuccess)
                    {
                        throw new Exception(Language.khong_tim_thay_tai_khoan);
                    }
                    else
                    {
                        //user
                        User = JsonConvert.DeserializeObject<User>(response.Content.ToString());

                        //otp
                        try
                        {
                            _otp = StringUtils.RandomString(4);
                            var mess = $"{_otp} {Language.la_ma_xac_thuc_cua_ban}";
                            await StringUtils.SendOTP(MaQuocGia.Value + Phone, mess);
                            MessagingCenter.Send<ForgetPasswordPageViewModel, bool>(this, "OtpPopup", true);
                        }
                        catch (Exception ex)
                        {
                            await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                        }
                        IsLoading = false;
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                    IsLoading = false;
                }
            }
            else
            {
                //check pwd & pwd confirm
                if (!Validations.IsValidPassword(Password))
                {
                    await Application.Current.MainPage.DisplayAlert("", Language.mat_khau_so_ky_tu, Language.dong);
                    IsLoading = false;
                    return;
                }
                if (Password != Confirm)
                {
                    await Application.Current.MainPage.DisplayAlert("", Language.xac_nhan_mat_khau_khong_dung, Language.dong);
                    IsLoading = false;
                    return;
                }

                //change pass
                try
                {
                    User.Password = Password;
                    var response = await ApiHelper.Put("api/user/forgetpassword", User);
                    if (response.IsSuccess)
                    {
                        await Application.Current.MainPage.DisplayAlert("", Language.doi_mat_khau_thanh_cong, Language.dong);
                        await Application.Current.MainPage.Navigation.PopAsync();
                        IsLoading = false;
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                    IsLoading = false;
                }

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

            IsPhoneVisible = false;
            IsChangePasswordVisible = true;
            MessagingCenter.Send<ForgetPasswordPageViewModel, bool>(this, "OtpPopup", false);

            IsLoading = false;
        }

        public async void ResetOTP()
        {
            _otp = StringUtils.RandomString(4);
            var mess = $"{_otp} {Language.la_ma_xac_thuc_cua_ban}";
            try
            {
                await StringUtils.SendOTP(Phone, mess);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
            }
        }
    }
}
