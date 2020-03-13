using System;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views;
using Xamarin.Auth;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class SocialLinkedPageViewModel : BaseViewModel
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

        private bool _isFacebookLinked;
        public bool IsFacebookLinked
        {
            get => _isFacebookLinked;
            set
            {
                _isFacebookLinked = value;
                OnPropertyChanged(nameof(IsFacebookLinked));
            }
        }

        private bool _isGoogleLinked;
        public bool IsGoogleLinked
        {
            get => _isGoogleLinked;
            set
            {
                _isGoogleLinked = value;
                OnPropertyChanged(nameof(IsGoogleLinked));
            }
        }

        private bool _isZaloLinked;
        public bool IsZaloLinked
        {
            get => _isZaloLinked;
            set
            {
                _isZaloLinked = value;
                OnPropertyChanged(nameof(IsZaloLinked));
            }
        }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }

        public bool fbFlag;
        public bool ggFlag;
        public bool zlFlag;

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

        private bool _isZaloLoginVisible;
        public bool IsZaloLoginVisible
        {
            get => _isZaloLoginVisible;
            set
            {
                _isZaloLoginVisible = value;
                OnPropertyChanged(nameof(IsZaloLoginVisible));
            }
        }

        public SocialLinkedPageViewModel(User user)
        {
            //_client = BsdHttpClient.Instance();
            User = user;

            if (User.FacebookId != -1 && !string.IsNullOrEmpty(User.FacebookId.ToString()))
            {
                fbFlag = true;
                IsFacebookLinked = true;
                Count++;
            }
            if (User.ZaloId != -1 && !string.IsNullOrEmpty(User.ZaloId.ToString()))
            {
                zlFlag = true;
                IsZaloLinked = true;
                Count++;
            }
            if (!string.IsNullOrEmpty(User.GoogleId))
            {
                ggFlag = true;
                IsGoogleLinked = true;
                Count++;
            }
        }

        public async void OnFacebookToggled()
        {
            if (IsFacebookLinked != fbFlag)
            {
                IsLoading = true;
                if (IsFacebookLinked)
                {
                    await FacebookHelper.FacebookLinked();
                    if (string.IsNullOrEmpty(UserLogged.FacebookId) || UserLogged.FacebookId == "-1")
                    {
                        IsFacebookLinked = false;
                        IsLoading = false;
                        return;
                    }
                    User.FacebookId = long.Parse(UserLogged.FacebookId);
                    Count++;
                    MessagingCenter.Send<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", User);
                    fbFlag = IsFacebookLinked;
                    IsLoading = false;
                }
                else
                {
                    if (Count < 2 && string.IsNullOrEmpty(User.Password))
                    {
                        IsFacebookLinked = true;
                        await Shell.Current.DisplayAlert("", Language.khong_the_huy_lien_ket, Language.dong);
                        IsLoading = false;
                        return;
                    }
                    var result = await Shell.Current.DisplayAlert("", Language.ban_co_chac_chan_muon_huy_lien_ket_khong, Language.dong_y, Language.huy);
                    if (result)
                    {
                        try
                        {
                            ApiResponse response = await ApiHelper.Put("api/auth/deletefacebook", null, true);
                            if (response.IsSuccess)
                            {
                                var user = JsonConvert.DeserializeObject<User>(response.Content.ToString());
                                User.FacebookId = user.FacebookId;
                                UserLogged.SaveProfile(User);
                                IsFacebookLinked = fbFlag = false;
                                Count--;
                                MessagingCenter.Send<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", User);
                                await Application.Current.MainPage.DisplayAlert("", Language.thanh_cong, Language.dong);
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
                    else
                    {
                        IsFacebookLinked = true;
                        IsLoading = false;
                    }
                }
            }
        }

        public async void OnGoogleToggled()
        {
            if (IsGoogleLinked != ggFlag)
            {
                IsLoading = true;
                if (IsGoogleLinked)
                {
                    //await GoogleHelper.GoogleLinked();

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

                    authenticator.Completed += OnAuthLinkedCompleted;
                    authenticator.Error += OnAuthLinkedError;

                    AuthenticationState.Authenticator = authenticator;

                    var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
                    presenter.Login(authenticator);

                }
                else
                {
                    if (Count < 2 && string.IsNullOrEmpty(User.Password))
                    {
                        IsGoogleLinked = true;
                        await Shell.Current.DisplayAlert("", Language.khong_the_huy_lien_ket, Language.dong);
                        IsLoading = false;
                        return;
                    }
                    var result = await Shell.Current.DisplayAlert("", Language.ban_co_chac_chan_muon_huy_lien_ket_khong, Language.dong_y, Language.huy);
                    if (result)
                    {
                        try
                        {
                            ApiResponse response = await ApiHelper.Put("api/auth/deletegoogle", null, true);
                            if (response.IsSuccess)
                            {
                                var user = JsonConvert.DeserializeObject<User>(response.Content.ToString());
                                User.GoogleId = user.GoogleId;
                                UserLogged.SaveProfile(User);
                                IsGoogleLinked = ggFlag = false;
                                Count--;
                                MessagingCenter.Send<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", User);
                                await Application.Current.MainPage.DisplayAlert("", Language.thanh_cong, Language.dong);
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
                    else
                    {
                        IsGoogleLinked = true;
                        IsLoading = false;
                    }
                }
            }
        }

        public async void OnZaloToggled()
        {
            if (IsZaloLinked != zlFlag)
            {
                IsLoading = true;
                if (IsZaloLinked)
                {
                    IsZaloLoginVisible = true;
                    IsLoading = false;
                    MessagingCenter.Subscribe<SocialLinkedPage, bool>(this, "UpdateSocialLinked", async (sender, arg) =>
                    {
                        if (string.IsNullOrEmpty(UserLogged.ZaloId) || UserLogged.ZaloId == "-1")
                        {
                            IsZaloLinked = false;
                            IsLoading = false;
                            return;
                        }
                        User.ZaloId = long.Parse(UserLogged.ZaloId);
                        Count++;
                        zlFlag = IsZaloLinked;
                        MessagingCenter.Send<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", User);
                        IsLoading = false;
                        return;
                    });
                }
                else
                {
                    if (Count < 2 && string.IsNullOrEmpty(User.Password))
                    {
                        IsZaloLinked = true;
                        await Shell.Current.DisplayAlert("", Language.khong_the_huy_lien_ket, Language.dong);
                        IsLoading = false;
                        return;
                    }
                    var result = await Shell.Current.DisplayAlert("", Language.ban_co_chac_chan_muon_huy_lien_ket_khong, Language.dong_y, Language.huy);
                    if (result)
                    {
                        try
                        {
                            ApiResponse response = await ApiHelper.Put("api/auth/deletezalo", null, true);
                            if (response.IsSuccess)
                            {
                                var user = JsonConvert.DeserializeObject<User>(response.Content.ToString());
                                User.ZaloId = user.ZaloId;
                                UserLogged.SaveProfile(User);
                                IsZaloLinked = zlFlag = false;
                                Count--;
                                MessagingCenter.Send<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", User);
                                await Application.Current.MainPage.DisplayAlert("", Language.thanh_cong, Language.dong);
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
                    else
                    {
                        IsZaloLinked = true;
                        IsLoading = false;
                    }
                }
            }
        }

        private void OnAuthLinkedError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthLinkedCompleted;
                authenticator.Error -= OnAuthLinkedError;
            }
            IsLoading = false;
            Debug.WriteLine("Authentication error: " + e.Message);
        }

        private async void OnAuthLinkedCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthLinkedCompleted;
                authenticator.Error -= OnAuthLinkedError;
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
                        AvatarUrl = googleUser.Picture
                    };

                    try
                    {
                        ApiResponse apiResponse = await ApiHelper.Put("api/auth/sociallinked", ggUser, true);
                        if (apiResponse.IsSuccess)
                        {
                            var user = JsonConvert.DeserializeObject<User>(apiResponse.Content.ToString());
                            UserLogged.SaveProfile(user);
                            User.GoogleId = UserLogged.GoogleId;
                            Count++;
                            ggFlag = IsGoogleLinked;
                            MessagingCenter.Send<SocialLinkedPageViewModel, User>(this, "UpdateSocialLinked", User);
                            await Application.Current.MainPage.DisplayAlert("", Language.lien_ket_google_thanh_cong, Language.dong);
                            IsLoading = false;
                        }
                        else
                        {
                            throw new Exception(apiResponse.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        IsGoogleLinked = false;
                        await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                        IsLoading = false;
                    }

                }
            }
            else
            {
                IsLoading = false;
                IsGoogleLinked = false;
            }
        }
    }
}
