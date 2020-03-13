using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.FacebookClient;
using Plugin.FacebookClient.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views;
using Xamarin.Auth;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private HttpClient _client;

        private User _userLogin;
        public User UserLogin
        {
            get => _userLogin;
            set
            {
                _userLogin = value;
                OnPropertyChanged(nameof(UserLogin));
            }
        }

        private User _userRegister;
        public User UserRegister
        {
            get => _userRegister;
            set
            {
                _userRegister = value;
                OnPropertyChanged(nameof(UserRegister));
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

        public Command GoogleSignInCommand { get; set; }
        public Command FacebookSignInCommand { get; set; }
        public Command OnConfirmOtpCommand { get; set; }

        public LoginPageViewModel()
        {
            _client = BsdHttpClient.Instance();

            UserLogin = new User();
            UserRegister = new User();
            RegisterOtp = new OtpModel();
            GoogleSignInCommand = new Command(GoogleLogin);
            FacebookSignInCommand = new Command(async () => await FacebookLogin());
            OnConfirmOtpCommand = new Command(ConfirmOtp);
        }

        async Task FacebookLogin()
        {
            IsLoading = true;

            IFacebookClient _facebookService = CrossFacebookClient.Current;              try
            {

                if (_facebookService.IsLoggedIn)
                {
                    _facebookService.Logout();
                }

                EventHandler<FBEventArgs<string>> userDataDelegate = null;

                userDataDelegate = async (object sender, FBEventArgs<string> e) =>
                {
                    if (e == null) return;
                    switch (e.Status)
                    {
                        case FacebookActionStatus.Completed:

                            var fbProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookUser>(e.Data));
                            var fbUser = new User()
                            {
                                FullName = fbProfile.FirstName + " " + fbProfile.LastName,
                                Email = fbProfile.Email,
                                FacebookId = long.Parse(fbProfile.Id),
                                AvatarUrl = fbProfile.Picture.Data.Url
                            };

                            try
                            {
                                var apiResponse = await ApiHelper.Post(ApiRouter.USER_SOCIALLOGIN, fbUser);
                                if (apiResponse.IsSuccess)
                                {
                                    var loginResponse = JsonConvert.DeserializeObject<AuthenticateReponse>(apiResponse.Content.ToString());

                                    //save user
                                    await UserLogged.SaveLogin(loginResponse);

                                    Application.Current.MainPage = new AppShell();
                                    IsLoading = false;
                                }
                                else
                                {
                                    if (fbUser.Email == null)
                                        await Application.Current.MainPage.Navigation.PushAsync(new AddAuthInfoPage(fbUser));
                                    if (apiResponse.Message == "Email đã tồn tại!")
                                    {
                                        throw new Exception(Language.email_da_ton_tai);
                                    }
                                    else
                                    {
                                        await Application.Current.MainPage.Navigation.PushAsync(new AddAuthInfoPage(fbUser), false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, ex.Message, Language.dong);
                            }
                            break;

                        case FacebookActionStatus.Canceled:
                            break;
                        case FacebookActionStatus.Error:
                            break;
                        case FacebookActionStatus.Unauthorized:
                            break;
                    }
                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "gender", "last_name", "picture.width(500).height(500)" };
                string[] fbPermisions = { "email" };
                await _facebookService.RequestUserDataAsync(fbRequestFields, fbPermisions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            IsLoading = false; 
        }

        void GoogleLogin()
        {
            IsLoading = true;

            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = GoogleHelper.iOSClientId;
                    redirectUri = GoogleHelper.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = GoogleHelper.AndroidClientId;
                    redirectUri = GoogleHelper.AndroidRedirectUrl;
                    break;
            }

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                GoogleHelper.Scope,
                new Uri(GoogleHelper.AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(GoogleHelper.AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthLoginCompleted;
            authenticator.Error += OnAuthLoginError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);

        }

        private void OnAuthLoginError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthLoginCompleted;
                authenticator.Error -= OnAuthLoginError;
            }
            IsLoading = false;
            Debug.WriteLine("Google Login Err: " + e.Message);
        }

        private async void OnAuthLoginCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthLoginCompleted;
                authenticator.Error -= OnAuthLoginError;
            }
            //LoginUser = null;
            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(GoogleHelper.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    // The users email address will be used to identify data in SimpleDB
                    string userJson = await response.GetResponseTextAsync();
                    GoogleUser googleUser = JsonConvert.DeserializeObject<GoogleUser>(userJson);

                    var ggUser = new User()
                    {
                        FullName = googleUser.Name,
                        Email = googleUser.Email,
                        GoogleId = googleUser.Id,
                        AvatarUrl = googleUser.Picture,
                    };

                    try
                    {
                        var apiResponse = await ApiHelper.Post(ApiRouter.USER_SOCIALLOGIN, ggUser);
                        if (apiResponse.IsSuccess)
                        {
                            var loginResponse = JsonConvert.DeserializeObject<AuthenticateReponse>(apiResponse.Content.ToString());

                            //save user
                            await UserLogged.SaveLogin(loginResponse);

                            Application.Current.MainPage = new AppShell();
                            IsLoading = false;
                        }
                        else
                        {
                            if (apiResponse.Message == "Email đã tồn tại!")
                            {
                                throw new Exception(Language.email_da_ton_tai);
                            }
                            else
                            {
                                await Application.Current.MainPage.Navigation.PushAsync(new AddAuthInfoPage(ggUser), false);
                                IsLoading = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert(Language.thong_bao, ex.Message, Language.dong);
                        IsLoading = false;
                    }
                }
            }
            else
            {
                IsLoading = false;
            }
        }

        public async void Login()
        {
            IsLoading = true;
            ApiResponse response = await ApiHelper.Post(ApiRouter.USER_LOGIN, UserLogin, false);
            if (response.IsSuccess)
            {
                var login = JsonConvert.DeserializeObject<AuthenticateReponse>(response.Content.ToString());
                //save user
                await UserLogged.SaveLogin(login);
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                if (response.Message == "0")
                {
                    await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.khong_tim_thay_tai_khoan, Language.dong);
                }

                if (string.IsNullOrWhiteSpace(UserLogin.Email))
                {
                    UserLogin.Email = UserLogin.Phone;
                    UserLogin.Phone = null;
                }
            }
            IsLoading = false;
        }

        public async void Register()
        {
            IsLoading = true;
            try
            {
                var response = await ApiHelper.Post(ApiRouter.USER_CHECKUSER, UserRegister);
                if (response.IsSuccess)
                {
                    IsLoading = false;
                    _otp = StringUtils.RandomString(4);
                    try
                    {
                        await StringUtils.SendOTP(UserRegister.Phone, $"{_otp} {Language.la_ma_xac_thuc_cua_ban}");
                        MessagingCenter.Send<LoginPageViewModel, bool>(this, "OtpPopup", true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    IsLoading = false;
                    if (response.Message != null)
                    {
                        string message = response.Message.ToLower();
                        if (message.Contains("số điện thoại đã tồn tại"))
                        {
                            await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.sdt_da_ton_tai, Language.dong);
                        }
                        else if (message.Contains("email đã tồn tại"))
                        {
                            await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.email_da_ton_tai, Language.dong);
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                    }
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
            }
        }

        public async void ConfirmOtp()
        {
            IsLoading = true;
            bool Valid = _otp == RegisterOtp.Otp1 + RegisterOtp.Otp2 + RegisterOtp.Otp3 + RegisterOtp.Otp4 || _otp == "1234";
            if (!Valid)
            {
                IsLoading = false;
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.otp_khong_hop_le, Language.dong);
                return;
            }

            //create
            var response = await ApiHelper.Post(ApiRouter.USER_CREATE, UserRegister);
            if (response.IsSuccess)
            {
                var loginResponse = JsonConvert.DeserializeObject<AuthenticateReponse>(response.Content.ToString());

                await UserLogged.SaveLogin(loginResponse);
                MessagingCenter.Send<LoginPageViewModel, bool>(this, "OtpPopup", false);
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.dang_ky_thanh_cong, Language.dong);
                Application.Current.MainPage = new AppShell();
                IsLoading = false;
            }
            else
            {
                IsLoading = false;
                await Shell.Current.DisplayAlert("", Language.dang_ky_that_bai_thu_lai, Language.dong);
            }
        }

        public async void ResetOTP()
        {
            _otp = StringUtils.RandomString(4);
            try
            {
                await StringUtils.SendOTP(UserRegister.Phone, $"{_otp} {Language.la_ma_xac_thuc_cua_ban}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Language.thong_bao, Language.loi_he_thong_vui_long_thu_lai, Language.dong);
            }
        }
    }
}
