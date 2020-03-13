using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.FacebookClient;
using Plugin.FacebookClient.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.Helpers
{
    public static class FacebookHelper
    {

        private static HttpClient _client = BsdHttpClient.Instance();

        static IFacebookClient _facebookService = CrossFacebookClient.Current;

        public static async Task FacebookLinked()
        {
            try
            {

                if (_facebookService.IsLoggedIn)
                {
                    _facebookService.Logout();
                }

                EventHandler<FBEventArgs<string>> userDataDelegate = null;

                userDataDelegate = async (object sender, FBEventArgs<string> e) =>
                {
                    if (e == null) return;
                    //switch (e.Status)
                    //{
                    //    case FacebookActionStatus.Completed:
                    //        break;
                    //    case FacebookActionStatus.Canceled:
                    //        break;
                    //}
                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "gender", "last_name", "picture.width(500).height(500)" };
                string[] fbPermisions = { "email" };
                var res = await _facebookService.RequestUserDataAsync(fbRequestFields, fbPermisions);
                var fbProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookUser>(res.Data));
                var fbUser = new User()
                {
                    FullName = fbProfile.FirstName + " " + fbProfile.LastName,
                    Email = fbProfile.Email,
                    FacebookId = long.Parse(fbProfile.Id),
                    AvatarUrl = fbProfile.Picture.Data.Url
                };

                //check exist
                try
                {
                    ApiResponse response = await ApiHelper.Put("api/auth/sociallinked", fbUser, true);
                    if (response.IsSuccess)
                    {
                        var user = JsonConvert.DeserializeObject<User>(response.Content.ToString());
                        UserLogged.SaveProfile(user);
                        await Application.Current.MainPage.DisplayAlert("Thông báo", Language.lien_ket_facebook_thanh_cong, Language.dong);
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Thông báo", ex.Message, Language.dong);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public static async Task ShareDataAsync(string url)
        {
            var loginResponse = await CrossFacebookClient.Current.LoginAsync(new string[] { "email" });

            if (loginResponse.Status != FacebookActionStatus.Completed)
            {
                return;
            }
            else
            {
                var res = await CrossFacebookClient.Current.ShareAsync(new FacebookShareLinkContent(null, new Uri(url)));
                switch (res.Status)
                {
                    case FacebookActionStatus.Completed:
                        await Shell.Current.DisplayAlert("", Language.chia_se_thanh_cong, Language.dong);
                        return;
                    case FacebookActionStatus.Canceled:
                        //await Shell.Current.DisplayAlert("Thông báo", "Huỷ chia sẻ tin!", Language.dong);
                        return;

                    case FacebookActionStatus.Unauthorized:
                        await Shell.Current.DisplayAlert("", Language.loi_xac_thuc, Language.dong);
                        return;

                    case FacebookActionStatus.Error:
                        await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                        return;
                }
            }
        }

    }
}