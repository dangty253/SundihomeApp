using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class SocialLinkedPage : ContentPage
    {
        public long ZALOAPPID = 1758025901124001;
        public string CallBackUrl = "https://bsdinsight.net/api/auth/getzalocode";
        public string AppScret = "BMjIc4t2rX6WoDSl2VgF";

        private static HttpClient _client = BsdHttpClient.Instance();

        public SocialLinkedPageViewModel viewModel;
        private User _user;
        public SocialLinkedPage(User user)
        {
            InitializeComponent();
            _user = user;
            BindingContext = viewModel = new SocialLinkedPageViewModel(_user);

            DependencyService.Get<IClearCookies>().Clear();
            webView.Source = $"https://oauth.zaloapp.com/v3/auth?app_id={ZALOAPPID}&redirect_uri={CallBackUrl}&state=123";

            webView.Navigated += WebView_NavigatingSocialLinked;
        }

        void OnFacebookToggled(object sender, ToggledEventArgs e)
        {
            viewModel.OnFacebookToggled();
        }

        void OnGoogleToggled(object sender, ToggledEventArgs e)
        {
            viewModel.OnGoogleToggled();
        }

        void OnZaloToggled(object sender, ToggledEventArgs e)
        {
            viewModel.OnZaloToggled();
        }

        void OnClosePopup(object sender, EventArgs e)
        {
            viewModel.IsZaloLoginVisible = false;
            viewModel.zlFlag = viewModel.IsZaloLinked = false;
        }

        private async void WebView_NavigatingSocialLinked(object sender, WebNavigatedEventArgs e)
        {
            //gridLoading.IsVisible = false;
            if (e.Url.StartsWith("https://bsdinsight.net", StringComparison.Ordinal))
            {
                try
                {
                    string queryString = e.Url.Replace(CallBackUrl, "");
                    Dictionary<string, string> keyValues = ParseQueryString(queryString);
                    if (keyValues.ContainsKey("code") == false) return;

                    string code = keyValues["code"];
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync($"https://oauth.zaloapp.com/v3/access_token?app_id={ZALOAPPID}&app_secret={AppScret}&code={code}");
                        if (response.IsSuccessStatusCode)
                        {
                            HttpContent auContent = response.Content;
                            string authBody = await auContent.ReadAsStringAsync();
                            ZaloAuthReponse zaloAuthReponse = JsonConvert.DeserializeObject<ZaloAuthReponse>(authBody);


                            var profileResponse = await client.GetAsync($"https://graph.zalo.me/v2.0/me?access_token={zaloAuthReponse.access_token}&fields=id,birthday,name,gender,picture");
                            var profileBody = await profileResponse.Content.ReadAsStringAsync();
                            ZaloUser zaloUser = JsonConvert.DeserializeObject<ZaloUser>(profileBody);

                            var zlUser = new User()
                            {
                                ZaloId = long.Parse(zaloUser.id),
                                FullName = zaloUser.name,
                                Sex = zaloUser.gender == "male" ? (short)0 : zaloUser.gender == "female" ? (short)1 : (short)-1,
                                Birthday = DateTime.ParseExact(zaloUser.birthday, "dd/MM/yyyy", null),
                                AvatarUrl = zaloUser.picture.data.url
                            };

                            try
                            {
                                ApiResponse apiResponse = await ApiHelper.Put("api/auth/sociallinked", zlUser, true);
                                if (apiResponse.IsSuccess)
                                {
                                    var user = JsonConvert.DeserializeObject<User>(apiResponse.Content.ToString());
                                    UserLogged.SaveProfile(user);
                                    viewModel.IsZaloLoginVisible = false;
                                    await Application.Current.MainPage.DisplayAlert("", Language.lien_ket_zalo_thanh_cong, Language.dong);
                                    MessagingCenter.Send<SocialLinkedPage, bool>(this, "UpdateSocialLinked", true);
                                }
                                else
                                {
                                    throw new Exception(apiResponse.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                await Application.Current.MainPage.DisplayAlert("", ex.Message, Language.dong);
                                await Navigation.PopAsync();
                            }
                        }
                        else
                        {
                            await DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                            await this.Navigation.PopToRootAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                    await this.Navigation.PopToRootAsync();
                }
            }
        }

        public Dictionary<string, string> ParseQueryString(String query)
        {
            Dictionary<String, String> queryDict = new Dictionary<string, string>();
            foreach (String token in query.TrimStart(new char[] { '?' }).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                    queryDict[parts[0].Trim()] = HttpUtility.UrlDecode(parts[1]).Trim();
                else
                    queryDict[parts[0].Trim()] = "";
            }
            return queryDict;
        }
    }
}
