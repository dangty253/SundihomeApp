using System;
using Plugin.FacebookClient;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;
using SundihomeApp.Views.MoiGioiViews;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using SundihomeApp.Resources;
using SundihomeApp.Views.BankViews;

namespace SundihomeApp.Views
{
    public partial class AccountPage : ContentPage
    {
        private bool HadInitilize = false;
        public AccountPageViewModel viewModel;
        public AccountPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (HadInitilize == true) return;
            HadInitilize = true;

            BindingContext = viewModel = new AccountPageViewModel();

            // upate tu profile page.
            MessagingCenter.Subscribe<ProfilePage, string>(this, "UpdateAvatar", (sender, avatar) =>
            {
                viewModel.User.AvatarUrl = avatar;
                image.Source = viewModel.User.AvatarFullUrl;
            });

            MessagingCenter.Subscribe<ProfilePageViewModel, User>(this, "UpdateProfile", (sender, user) =>
            {
                viewModel.User = user;
                image.Source = viewModel.User.AvatarFullUrl;
            });

            MessagingCenter.Subscribe<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", async (sender, arg) =>
            {
                viewModel.User = arg;
                viewModel.SocialCount = 0;
                if (viewModel.User.FacebookId != -1 && !string.IsNullOrEmpty(viewModel.User.FacebookId.ToString()))
                    viewModel.SocialCount++;
                if (viewModel.User.ZaloId != -1 && !string.IsNullOrEmpty(viewModel.User.ZaloId.ToString()))
                    viewModel.SocialCount++;
                if (!string.IsNullOrEmpty(viewModel.User.GoogleId))
                    viewModel.SocialCount++;
            });

            //LabelLanguage.Text = AppShell.Languages[LanguageSettings.Language];

        }

        // edit profile
        private void OnEditProfileClicked(object sender, EventArgs e)
        {
            User user = new User()
            {
                Id = viewModel.User.Id,
                Email = viewModel.User.Email,
                Phone = viewModel.User.Phone,
                Password = viewModel.User.Password,
                FullName = viewModel.User.FullName,
                Sex = viewModel.User.Sex,
                Birthday = viewModel.User.Birthday,
                Address = viewModel.User.Address,
                AvatarUrl = viewModel.User.AvatarUrl,
                FacebookId = viewModel.User.FacebookId,
                GoogleId = viewModel.User.GoogleId,
                ZaloId = viewModel.User.ZaloId,
                ProvinceId = viewModel.User.ProvinceId,
                DistrictId = viewModel.User.DistrictId,
                WardId = viewModel.User.WardId,
                Street = viewModel.User.Street
            };

            if (string.IsNullOrEmpty(UserLogged.CompanyId))
            {
                user.CompanyId = null;
            }
            else
            {
                user.CompanyId = Guid.Parse(UserLogged.CompanyId);
            }

            Navigation.PushAsync(new ProfilePage(user));
        }

        // goto social linked page
        void OnSocialLinkedClicked(object sender, EventArgs e)
        {
            User user = new User()
            {
                Id = viewModel.User.Id,
                Email = viewModel.User.Email,
                Phone = viewModel.User.Phone,
                Password = viewModel.User.Password,
                FullName = viewModel.User.FullName,
                Sex = viewModel.User.Sex,
                Birthday = viewModel.User.Birthday,
                Address = viewModel.User.Address,
                AvatarUrl = viewModel.User.AvatarUrl,
                FacebookId = viewModel.User.FacebookId,
                GoogleId = viewModel.User.GoogleId,
                ZaloId = viewModel.User.ZaloId
            };

            if (string.IsNullOrEmpty(UserLogged.CompanyId))
            {
                user.CompanyId = null;
            }
            else
            {
                user.CompanyId = Guid.Parse(UserLogged.CompanyId);
            }

            Navigation.PushAsync(new SocialLinkedPage(user));
        }

        // goto company page
        private async void OnCompanyClicked(object sender, EventArgs e)
        {
            await UserLogged.ReloadProfile();
            var appShell = (AppShell)Shell.Current;
            appShell.AddMenu_QuanLyCongTy();

            if (string.IsNullOrEmpty(UserLogged.CompanyId))
            {
                string action = await DisplayActionSheet(Language.cong_ty, Language.huy, null, Language.dang_ky_cong_ty, Language.dang_ky_nhan_vien);
                if (action == Language.dang_ky_cong_ty)
                {
                    await Navigation.PushAsync(new AddCompanyPage());
                }
                else if (action == Language.dang_ky_nhan_vien)
                {
                    enNumPhone.Text = "";
                    await ModalRegisterEmployee.Show();
                }
            }
            else
            {
                await Shell.Current.GoToAsync("//" + AppShell.QUANLYCONGTY);
            }
        }


        // goto posted list page
        private void OnPostedListClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MyPostListPage());
        }

        private async void OnMyProjectListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProjectListPage(true));
        }

        // goto post saved list page
        private void OnPostSavedListClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MyFavoritePostListPage());
        }

        // goto follower list page
        private void OnFollowerListClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserFollowPage(false, viewModel.FollowerList) { Title = Language.nguoi_theo_doi });
        }

        // goto following list page
        private void OnFollowingListClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserFollowPage(true, viewModel.FollowingList) { Title = Language.dang_theo_doi });
        }

        // goto report list page
        private void OnReportClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ReportPage());
        }

        private async void ViewAppointment_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AppointmentListPage());
        }

        // logout
        async void OnLogoutClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert(Language.thong_bao, Language.ban_co_chac_chan_muon_dang_xuat, Language.dong_y, Language.huy);
            if (!result) return;

            string language = LanguageSettings.Language;

            await ApiHelper.Post(ApiRouter.USER_LOGGOUT, null, true);
            UserLogged.Logout();

            App.SetCultureInfo(language);
            Application.Current.MainPage = new AppShell();
        }

        // moi gioi	
        private async void OpenModalDangKyMoiGioi_Tapped(object sender, EventArgs e)
        {
            await UserLogged.ReloadProfile();
            var appShell = (AppShell)Shell.Current;
            appShell.AddMenu_QuanLyMoiGioi();

            if (UserLogged.Type == 1)
            {
                await Shell.Current.GoToAsync("//" + AppShell.QUANLYMOIGIOI);
            }
            else
            {
                await DangKyMoiGioi();
            }
        }
        private async Task DangKyMoiGioi()
        {
            if (ModalDangKyMoiGioi.Body == null)
            {
                var dangKyMoiGioiContentView = new DangKyMoiGioiContentView(LookUpModal, Guid.Parse(UserLogged.Id));
                dangKyMoiGioiContentView.OnSaved += async (object sender, EventArgs e) =>
                {
                    await ModalDangKyMoiGioi.Hide();
                    await Shell.Current.GoToAsync("//" + AppShell.QUANLYMOIGIOI);
                };
                dangKyMoiGioiContentView.OnCancel += async (object sender, EventArgs e) => await ModalDangKyMoiGioi.Hide();
                ModalDangKyMoiGioi.Body = dangKyMoiGioiContentView;
                ModalDangKyMoiGioi.CustomCloseButton(dangKyMoiGioiContentView.Cancel_Clicked);

            }
            await ModalDangKyMoiGioi.Show();
        }

        public async void ChangeLanguage_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet(Language.chon_ngon_ngu, Language.huy, null, AppShell.Languages.Values.ToArray());

            if (result == AppShell.Languages["vi"])
            {
                App.SetCultureInfo("vi");
                Application.Current.MainPage = new AppShell();
            }
            else if (result == AppShell.Languages["en"])
            {
                App.SetCultureInfo("en");
                Application.Current.MainPage = new AppShell();
            }
        }

        private async void GoiVay_Clicked(object sender, EventArgs e)
        {
            var response = await ApiHelper.Post(ApiRouter.BANK_EMPLOYEE_CHECK, null, true);
            if (response.IsSuccess && response.Content != null)
            {
                bool isEmployee = (bool)response.Content;
                if (isEmployee)
                {
                    // chuyen qua man hinh danh sach goi vay
                    await Navigation.PushAsync(new MyGoiVayListPage());
                }
                else
                {
                    await DisplayAlert("", Language.dang_ky_nhan_vien_ngan_hang, Language.dong);
                    var dangKyNhanVienView = new DangKyNhanVienNganHangView(LookUpModal);
                    dangKyNhanVienView.OnCancel += async (s, e2) => await ModalBankEmployeeRegister.Hide();
                    dangKyNhanVienView.OnSaved += async (s, e2) =>
                    {
                        await ModalBankEmployeeRegister.Hide();
                        await Navigation.PushAsync(new MyGoiVayListPage());
                        ToastMessageHelper.ShortMessage(Language.dang_ky_thanh_cong);
                    };
                    ModalBankEmployeeRegister.Body = dangKyNhanVienView;
                    await ModalBankEmployeeRegister.Show();
                }
            }
        }

        private async void OnClick_SendConFirm(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(enNumPhone.Text))
            {
                await DisplayAlert("", Language.vui_long_nhap_so_dien_thoai, Language.dong);
                return;
            }

            ICompanyService companyService = DependencyService.Get<ICompanyService>();
            InviteUser inviteUser = companyService.FindInviteUser(UserLogged.Phone, enNumPhone.Text);

            if (inviteUser == null)
            {
                await DisplayAlert("", Language.otp_khong_hop_le, Language.dong);
                return;
            }

            ApiResponse apiResponse = await ApiHelper.Get<User>($"{ApiRouter.EMPLOYEE_GET_EMPLOYEE_BY_PHONE}/" + inviteUser.PhoneNumber, true);
            if (apiResponse.Content != null)
            {
                User user = apiResponse.Content as User;
                user.CompanyId = Guid.Parse(inviteUser.CompanyId);
                ApiResponse responseUser = await ApiHelper.Put($"{ApiRouter.EMPLOYEE_CONFIRM_COMPANNY}", user, true);
                if (responseUser.IsSuccess)
                {
                    companyService.DeleteInvitedUser(inviteUser.Id);// delete inviteruser khi da xac nhan
                    UserLogged.CompanyId = inviteUser.CompanyId;
                    await ModalRegisterEmployee.Hide();
                    enNumPhone.Text = null;

                    this.OnCompanyClicked(sender, e);
                    ToastMessageHelper.ShortMessage(Language.dang_ky_thanh_cong);
                }
            }
        }
    }
}
