using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views;
using Xamarin.Auth;
using Xamarin.Forms;

namespace SundihomeApp.Helpers
{
    public static class GoogleHelper
    {
        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public const string iOSClientId = "607579750145-7jckghtgn6mhqva1o7762ouceng4mh1k.apps.googleusercontent.com";
        public const string AndroidClientId = "607579750145-5h06oc56djfv2imtbejd9sartht4619a.apps.googleusercontent.com";

        // These values do not need changing
        public const string Scope = "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/plus.login";
        public const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public const string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public const string iOSRedirectUrl = "com.googleusercontent.apps.607579750145-7jckghtgn6mhqva1o7762ouceng4mh1k:/oauth2redirect";
        public const string AndroidRedirectUrl = "com.googleusercontent.apps.607579750145-5h06oc56djfv2imtbejd9sartht4619a:/oauth2redirect";

        private static HttpClient _client = BsdHttpClient.Instance();

        public static void GoogleLogin()
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = iOSClientId;
                    redirectUri = iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = AndroidClientId;
                    redirectUri = AndroidRedirectUrl;
                    break;
            }

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Scope,
                new Uri(AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthLoginCompleted;
            authenticator.Error += OnAuthLoginError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        private static void OnAuthLoginError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthLoginCompleted;
                authenticator.Error -= OnAuthLoginError;
            }

            Debug.WriteLine("Authentication error: " + e.Message);
        }

        private static async void OnAuthLoginCompleted(object sender, AuthenticatorCompletedEventArgs e)
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
                var request = new OAuth2Request("GET", new Uri(UserInfoUrl), null, e.Account);
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

                    var uri = new Uri(ApiConfig.IP + ApiRouter.USER_SOCIALLOGIN);
                    try
                    {
                        var json = JsonConvert.SerializeObject(ggUser);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage httpResponse = await _client.PostAsync(uri, content);
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            var loginResponseJson = await httpResponse.Content.ReadAsStringAsync();
                            var loginResponse = JsonConvert.DeserializeObject<AuthenticateReponse>(loginResponseJson);

                            //save user
                            UserLogged.SaveLogin(loginResponse);

                            Application.Current.MainPage = new AppShell();
                        }
                        else
                        {
                            var message = JsonConvert.DeserializeObject<string>(await httpResponse.Content.ReadAsStringAsync());
                            if (message == "Email đã tồn tại!")
                            {
                                throw new Exception(Language.email_da_ton_tai);
                            }
                            else
                            {
                                await Application.Current.MainPage.Navigation.PushAsync(new AddAuthInfoPage(ggUser), false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Thông báo", ex.Message, Language.dong);
                    }

                }
            }
        }

        public static async Task GoogleLinked()
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = iOSClientId;
                    redirectUri = iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = AndroidClientId;
                    redirectUri = AndroidRedirectUrl;
                    break;
            }

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Scope,
                new Uri(AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthLinkedCompleted;
            authenticator.Error += OnAuthLinkedError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        private static void OnAuthLinkedError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthLinkedCompleted;
                authenticator.Error -= OnAuthLinkedError;
            }

            Debug.WriteLine("Authentication error: " + e.Message);
        }

        private static async void OnAuthLinkedCompleted(object sender, AuthenticatorCompletedEventArgs e)
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
                var request = new OAuth2Request("GET", new Uri(UserInfoUrl), null, e.Account);
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
                            await Application.Current.MainPage.DisplayAlert("", Language.lien_ket_google_thanh_cong, Language.dong);
                            //MessagingCenter.Send<GoogleHelper, bool>(this, "UpdateSocialLinked", true);
                        }
                        else
                        {
                            throw new Exception(apiResponse.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                    }

                }
            }
        }
    }

    public class AuthenticationState
    {
        public static OAuth2Authenticator Authenticator;
    }


}
