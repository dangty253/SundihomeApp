using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class ZaloLoginPage : ContentPage
    {
        public long ZALOAPPID = 1758025901124001;
        public string CallBackUrl = "https://bsdinsight.net/api/auth/getzalocode";
        public string AppScret = "BMjIc4t2rX6WoDSl2VgF";

        private static HttpClient _client = BsdHttpClient.Instance();

        public ZaloLoginPage()
        {
            InitializeComponent();
            DependencyService.Get<IClearCookies>().Clear();
            webView.Source = $"https://oauth.zaloapp.com/v3/auth?app_id={ZALOAPPID}&redirect_uri={CallBackUrl}&state=123";
            webView.Navigated += WebView_Navigating;
        }

        private async void WebView_Navigating(object sender, WebNavigatedEventArgs e)
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
                                var apiResponse = await ApiHelper.Post(ApiRouter.USER_SOCIALLOGIN, zlUser);
                                if (apiResponse.IsSuccess)
                                {
                                    var loginResponse = JsonConvert.DeserializeObject<AuthenticateReponse>(apiResponse.Content.ToString());

                                    //save user
                                    await UserLogged.SaveLogin(loginResponse);

                                    Application.Current.MainPage = new AppShell();
                                }
                                else
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new AddAuthInfoPage(zlUser));
                                }
                            }
                            catch (Exception ex)
                            {
                                await Application.Current.MainPage.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
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
