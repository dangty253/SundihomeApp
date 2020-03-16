using System;
using System.Threading.Tasks;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.Settings
{
    public class UserLogged
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string Id
        {
            get => AppSettings.GetValueOrDefault(nameof(Id), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Id), value);
        }

        public static string Password
        {
            get => AppSettings.GetValueOrDefault(nameof(Password), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Password), value);
        }

        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault(nameof(AccessToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AccessToken), value);
        }

        public static string FullName
        {
            get => AppSettings.GetValueOrDefault(nameof(FullName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FullName), value);
        }

        public static string Email
        {
            get => AppSettings.GetValueOrDefault(nameof(Email), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Email), value);
        }

        public static string Phone
        {
            get => AppSettings.GetValueOrDefault(nameof(Phone), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Phone), value);
        }

        public static DateTime Birthday
        {
            get => AppSettings.GetValueOrDefault(nameof(Birthday), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(Birthday), value);
        }

        public static int Sex
        {
            get => AppSettings.GetValueOrDefault(nameof(Sex), -1);
            set => AppSettings.AddOrUpdateValue(nameof(Sex), value);
        }

        public static int ProvinceId
        {
            get => AppSettings.GetValueOrDefault(nameof(ProvinceId), -1);
            set => AppSettings.AddOrUpdateValue(nameof(ProvinceId), value);
        }

        public static int DistrictId
        {
            get => AppSettings.GetValueOrDefault(nameof(DistrictId), -1);
            set => AppSettings.AddOrUpdateValue(nameof(DistrictId), value);
        }

        public static int WardId
        {
            get => AppSettings.GetValueOrDefault(nameof(WardId), -1);
            set => AppSettings.AddOrUpdateValue(nameof(WardId), value);
        }

        public static string Street
        {
            get => AppSettings.GetValueOrDefault(nameof(Street), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Street), value);
        }

        public static string Address
        {
            get => AppSettings.GetValueOrDefault(nameof(Address), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Address), value);
        }

        public static string AvatarUrl
        {
            get => AppSettings.GetValueOrDefault(nameof(AvatarUrl), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AvatarUrl), value);
        }

        public static DateTime RegisterDate
        {
            get => AppSettings.GetValueOrDefault(nameof(RegisterDate), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(RegisterDate), value);
        }

        public static string FacebookId
        {
            get => AppSettings.GetValueOrDefault(nameof(FacebookId), "-1");
            set => AppSettings.AddOrUpdateValue(nameof(FacebookId), value);
        }

        public static string GoogleId
        {
            get => AppSettings.GetValueOrDefault(nameof(GoogleId), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(GoogleId), value);
        }

        public static string ZaloId
        {
            get => AppSettings.GetValueOrDefault(nameof(ZaloId), "-1");
            set => AppSettings.AddOrUpdateValue(nameof(ZaloId), value);
        }

        public static string CompanyId
        {
            get => AppSettings.GetValueOrDefault(nameof(CompanyId), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CompanyId), value);
        }

        public static string FirebaseRegToken
        {
            get => AppSettings.GetValueOrDefault(nameof(FirebaseRegToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(FirebaseRegToken), value);
        }

        public static int RoleId
        {
            get => AppSettings.GetValueOrDefault(nameof(RoleId), -1);
            set => AppSettings.AddOrUpdateValue(nameof(RoleId), value);
        }

        public static int Type
        {
            get => AppSettings.GetValueOrDefault(nameof(Type), 0);
            set => AppSettings.AddOrUpdateValue(nameof(Type), value);
        }

        public static bool IsLogged
        {
            get => Id != string.Empty && AccessToken != string.Empty;
        }

        public static void SaveAvatar(string avatar)
        {
            AvatarUrl = avatar;
        }

        public static void SavePhone(string phone)
        {
            Phone = phone;
        }

        public static void SaveEmail(string email)
        {
            Email = email;
        }

        public static void SavePassword(string pwd)
        {
            Password = pwd;
        }

        public async static Task SaveLogin(AuthenticateReponse authResponse)
        {
            Id = authResponse.Id.ToString();
            AccessToken = authResponse.AccessToken;
            FullName = authResponse.FullName;
            Email = authResponse.Email;
            Phone = authResponse.Phone;
            Password = authResponse.Password;
            Address = authResponse.Address;
            RegisterDate = authResponse.RegisterDate.ToLocalTime();
            AvatarUrl = authResponse.AvatarUrl;
            Type = authResponse.Type;
            if (authResponse.RoleId.HasValue)
            {
                RoleId = authResponse.RoleId.Value;
            }
            else
            {
                RoleId = -1;
            }

            if (authResponse.Birthday.HasValue)
            {
                Birthday = authResponse.Birthday.Value.ToLocalTime();
            }

            if (authResponse.Sex.HasValue)
            {
                Sex = authResponse.Sex.Value;
            }

            if (authResponse.FacebookId.HasValue)
            {
                FacebookId = authResponse.FacebookId.Value.ToString();
            }
            if (authResponse.ZaloId.HasValue)
            {
                ZaloId = authResponse.ZaloId.Value.ToString();
            }
            GoogleId = authResponse.GoogleId;

            if (authResponse.CompanyId.HasValue)
            {
                CompanyId = authResponse.CompanyId.ToString();
            }
            else
            {
                CompanyId = null;
            }


            if (authResponse.ProvinceId.HasValue)
            {
                ProvinceId = authResponse.ProvinceId.Value;
            }
            else
            {
                ProvinceId = -1;
            }

            if (authResponse.DistrictId.HasValue)
            {
                DistrictId = authResponse.DistrictId.Value;
            }
            else
            {
                DistrictId = -1;
            }

            if (authResponse.WardId.HasValue)
            {
                WardId = authResponse.WardId.Value;
            }
            else
            {
                WardId = -1;
            }

            Street = authResponse.Street;

            await NotificationHelper.SaveToken();
        }


        public static void SaveProfile(User user)
        {
            FullName = user.FullName;
            Address = user.Address;
            if (user.Birthday.HasValue)
            {
                Birthday = user.Birthday.Value.ToLocalTime();
            }
            else
            {
                Birthday = DateTime.MinValue;
            }

            if (user.Sex.HasValue)
            {
                Sex = user.Sex.Value;
            }
            GoogleId = user.GoogleId;
            if (user.FacebookId.HasValue)
            {
                FacebookId = user.FacebookId.Value.ToString();
            }
            else
            {
                FacebookId = null;
            }
            if (user.ZaloId.HasValue)
            {
                ZaloId = user.ZaloId.Value.ToString();
            }
            else
            {
                ZaloId = null;
            }
            if (user.CompanyId.HasValue)
            {
                CompanyId = user.CompanyId.ToString();
            }

            //
            if (user.ProvinceId.HasValue)
            {
                ProvinceId = user.ProvinceId.Value;
            }
            else ProvinceId = -1;

            if (user.DistrictId.HasValue)
            {
                DistrictId = user.DistrictId.Value;
            }
            else DistrictId = -1;

            if (user.WardId.HasValue)
            {
                WardId = user.WardId.Value;
            }
            else WardId = -1;

            Street = user.Street;
        }

        public static async Task ReloadProfile()
        {
            if (UserLogged.IsLogged == false) return;
            var response = await ApiHelper.Get<User>(Configuration.ApiRouter.USER_GET_USER_BY_ID + "/" + UserLogged.Id);
            if (response.IsSuccess && response.Content != null)
            {
                var userData = response.Content as User;
                UserLogged.Type = userData.Type.HasValue ? userData.Type.Value : 0;
                UserLogged.RoleId = userData.RoleId.HasValue ? userData.RoleId.Value : -1;
                UserLogged.CompanyId = userData.CompanyId.HasValue ? userData.CompanyId.Value.ToString() : null;
            }
            else
            {
                UserLogged.Type = 0;
                UserLogged.RoleId = -1;
                UserLogged.CompanyId = null;
            }
        }

        public static void Logout()
        {
            AppSettings.Clear();
            Plugin.FacebookClient.CrossFacebookClient.Current.Logout();
        }
    }
}
