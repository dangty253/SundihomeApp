﻿using System;
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
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class AddAuthInfoPageViewModel : BaseViewModel
    {
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

        private bool _isEmailHasValue;
        public bool IsEmailHasValue
        {
            get => _isEmailHasValue;
            set
            {
                _isEmailHasValue = value;
                OnPropertyChanged(nameof(IsEmailHasValue));
            }
        }

        private bool _isPhoneHasValue;
        public bool IsPhoneHasValue
        {
            get => _isPhoneHasValue;
            set
            {
                _isPhoneHasValue = value;
                OnPropertyChanged(nameof(IsPhoneHasValue));
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

        public List<MaQuocGia> MaQuocGiaList { get; set; } = MaQuocGiaData.GetList();
        private MaQuocGia _maQuocGia;
        public MaQuocGia MaQuocGia
        {
            get => _maQuocGia; set
            {
                if (this._user != null)
                {
                    if (value != null)
                    {
                        this._user.MaQuocGia = value.Value;
                    }
                    else
                    {
                        this._user.MaQuocGia = null;
                    }
                }
                _maQuocGia = value;
                OnPropertyChanged(nameof(MaQuocGia));
            }
        }

        public Command SubmitCommand { get; set; }
        public Command OnConfirmOtpCommand { get; set; }

        public AddAuthInfoPageViewModel(User user)
        {
            User = user;
            RegisterOtp = new OtpModel();

            if (string.IsNullOrWhiteSpace(User.Email))
                IsEmailHasValue = true;
            if (string.IsNullOrWhiteSpace(User.Phone))
                IsPhoneHasValue = true;

            SubmitCommand = new Command(Submit);
            OnConfirmOtpCommand = new Command(ConfirmOtp);

            MaQuocGia = MaQuocGiaList[0];
        }

        public async void Submit()
        {
            IsLoading = true;
            //check valid
            if (IsEmailHasValue && !Validations.IsValidEmail(User.Email))
            {
                await Application.Current.MainPage.DisplayAlert("", Language.email_khong_hop_le, Language.dong);
                IsLoading = false;
                return;
            }
            if (IsPhoneHasValue && !Validations.IsValidPhone(User.Phone))
            {
                await Application.Current.MainPage.DisplayAlert("", Language.sdt_khong_hop_le, Language.dong);
                IsLoading = false;
                return;
            }

            User.MaQuocGia = MaQuocGia.Value;
            ApiResponse response = await ApiHelper.Post(ApiRouter.USER_CHECKUSER, User);
            if (response.IsSuccess)
            {
                IsLoading = false;
                _otp = StringUtils.RandomString(4);
                try
                {
                    await StringUtils.SendOTP(User.MaQuocGia + User.Phone, $"{_otp} " + Language.la_ma_xac_thuc_cua_ban);
                    MessagingCenter.Send<AddAuthInfoPageViewModel, bool>(this, "OtpPopup", true);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(Language.thong_bao, ex.Message, Language.dong);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, response.Message, Language.dong);
                IsLoading = false;
            }
        }

        public async void ConfirmOtp()
        {
            IsLoading = true;
            if (RegisterOtp.Otp1 + RegisterOtp.Otp2 + RegisterOtp.Otp3 + RegisterOtp.Otp4 != _otp)
            {
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.otp_khong_dung_vui_long_nhap_lai, Language.dong);
                IsLoading = false;
                return;
            }
            User.MaQuocGia = MaQuocGia.Value;
            ApiResponse response = await ApiHelper.Post(ApiRouter.USER_SOCIALLOGIN, User);
            if (response.IsSuccess)
            {
                var loginResponse = JsonConvert.DeserializeObject<AuthenticateReponse>(response.Content.ToString());
                await UserLogged.SaveLogin(loginResponse);
                MessagingCenter.Send<AddAuthInfoPageViewModel, bool>(this, "OtpPopup", false);
                IsLoading = false;
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.dang_ky_thanh_cong, Language.dong);
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                IsLoading = false;
                await Shell.Current.DisplayAlert("", Language.loi_vui_long_thu_lai, Language.dong);
            }
        }

        public async void ResetOTP()
        {
            try
            {
                _otp = StringUtils.RandomString(4);
                await StringUtils.SendOTP(User.MaQuocGia + User.Phone, $"{_otp} {Language.la_ma_xac_thuc_cua_ban}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, ex.Message, Language.dong);
            }
        }
    }
}