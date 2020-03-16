using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPageViewModel viewModel;

        string SelectedColor = "00579E";
        string UnselectedColor = "0089D1";

        static double updateRate = 1000 / 15f; // 30Hz
        static double step = updateRate / (2 * 15 * 1000f);

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new LoginPageViewModel();
            OtpPopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);

            MessagingCenter.Subscribe<LoginPageViewModel, bool>(this, "OtpPopup", (sender, arg) =>
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
                    spReset.TextColor = Color.FromHex(UnselectedColor);
                    return false;
                });
            });
        }

        void OnSignInTabbed(object sender, EventArgs e)
        {
            lblSignIn.TextColor = Color.FromHex(SelectedColor);
            bvSignIn.BackgroundColor = Color.FromHex(SelectedColor);
            svSignIn.IsVisible = true;

            lblSignUp.TextColor = Color.FromHex(UnselectedColor);
            bvSignUp.BackgroundColor = Color.White;
            svSignUp.IsVisible = false;
        }

        void OnSignUpTabbed(object sender, EventArgs e)
        {
            lblSignUp.TextColor = Color.FromHex(SelectedColor);
            bvSignUp.BackgroundColor = Color.FromHex(SelectedColor);
            svSignUp.IsVisible = true;

            lblSignIn.TextColor = Color.FromHex(UnselectedColor);
            bvSignIn.BackgroundColor = Color.White;
            svSignIn.IsVisible = false;
        }

        void OnForgetPassword(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ForgetPasswordPage());
        }

        public void OnLoginClicked(object sender, EventArgs e)
        {
            string email = SignInEmail.Text;
            bool hasError = false;
            if (!Validations.IsValidEmail(email) && !Validations.IsValidPhone(email))
            {
                InvalidSignInEmail.IsVisible = true;
                hasError = true;
            }

            if (!Validations.IsValidPassword(viewModel.UserLogin.Password))
            {
                InvalidSignInPassword.IsVisible = true;
                hasError = true;
            }
            if (hasError) return;

            InvalidSignInEmail.IsVisible = false;
            InvalidSignInPassword.IsVisible = false;

            if (Validations.IsValidEmail(email))
            {
                //dang nhap = email
                viewModel.UserLogin.Email = email;
                viewModel.UserLogin.Phone = null;
            }
            else if (Validations.IsValidPhone(email))
            {
                //dang nhap = sdt
                viewModel.UserLogin.Phone = email;
                viewModel.UserLogin.Email = null;
            }


            viewModel.Login();
        }

        public void OnSignUpClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(viewModel.UserRegister.FullName))
            {
                InvalidSignUpFullName.IsVisible = true;
            }

            if (viewModel.MaQuocGiaRegister.Code == "")
            {
                InvalidSignUpPhone.IsVisible = true;
                return;
            }

            if (!Validations.IsValidPhone(viewModel.UserRegister.Phone) || !Validations.IsValidEmail(viewModel.UserRegister.Email) || !Validations.IsValidPassword(viewModel.UserRegister.Password))
            {
                if (!Validations.IsValidPhone(viewModel.UserRegister.Phone))
                    InvalidSignUpPhone.IsVisible = true;
                if (!Validations.IsValidEmail(viewModel.UserRegister.Email))
                    InvalidSignUpEmail.IsVisible = true;
                if (!Validations.IsValidPassword(viewModel.UserRegister.Password))
                    InvalidSignUpPassword.IsVisible = true;
            }
            else
            {
                viewModel.Register();
            }
        }

        void OnSignInEmailChanged(object sender, TextChangedEventArgs e)
        {
            InvalidSignInEmail.IsVisible = false;
        }

        void OnSignInPasswordChanged(object sender, TextChangedEventArgs e)
        {
            InvalidSignInPassword.IsVisible = false;
        }

        void OnSignUpPhoneChanged(object sender, TextChangedEventArgs e)
        {
            InvalidSignUpPhone.IsVisible = false;
        }

        void OnSignUpEmailChanged(object sender, TextChangedEventArgs e)
        {
            InvalidSignUpEmail.IsVisible = false;
        }

        void OnSignUpPasswordChanged(object sender, TextChangedEventArgs e)
        {
            InvalidSignUpPassword.IsVisible = false;
        }

        void OnCloseLoginClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new AppShell();
        }

        void OnZaloSignIn(object sender, EventArgs e)
        {
            viewModel.IsLoading = true;
            Navigation.PushAsync(new ZaloLoginPage());
            viewModel.IsLoading = false;
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

        void OnCleanOTPButtonClicked(object sender, EventArgs e)
        {
            entryOTP1.Text = entryOTP2.Text = entryOTP3.Text = entryOTP4.Text = null;
            entryOTP1.Focus();
        }

        void OnResetOTP(object sender, EventArgs e)
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
                spReset.TextColor = Color.FromHex(UnselectedColor);
                return false;
            });
        }

        void RegisterFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidSignUpFullName.IsVisible = false;
        }

        void OpenPickerMaQuocGiaLogin_Tapped(object sender, EventArgs e)
        {
            PickerMaQuocGiaLogin.Focus();
        }
        void OpenPickerMaQuocGiaRegister_Tapped(object sender, EventArgs e)
        {
            PickerMaQuocGiaRegister.Focus();
        }
    }
}
