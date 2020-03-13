using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.Models;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class AddAuthInfoPage : ContentPage
    {
        public AddAuthInfoPageViewModel viewModel;
        private User _user;

        static double updateRate = 1000 / 15f; // 30Hz
        static double step = updateRate / (2 * 15 * 1000f);

        public AddAuthInfoPage(User user)
        {
            InitializeComponent();
            _user = user;
            BindingContext = viewModel = new AddAuthInfoPageViewModel(_user);

            OtpPopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);

            MessagingCenter.Subscribe<AddAuthInfoPageViewModel, bool>(this, "OtpPopup", async (sender, arg) =>
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
        }

        //OTP
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
                spReset.TextColor = Color.FromHex("0089D1");
                return false;
            });
        }

        void OnClosePopup(object sender, EventArgs e)
        {
            OtpPopup.IsVisible = false;
            progressBar.Progress = 0;
            OnCleanOTPButtonClicked(sender, e);
        }
    }
}
