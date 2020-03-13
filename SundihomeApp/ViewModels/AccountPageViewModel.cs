using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class AccountPageViewModel : BaseViewModel
    {
        private HttpClient _client;

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

        private int _socialCount;
        public int SocialCount
        {
            get => _socialCount;
            set
            {
                _socialCount = value;
                OnPropertyChanged(nameof(SocialCount));
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

        private Company _company;
        public Company Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged(nameof(Company));
            }
        }

        private string _mst;
        public string MST
        {
            get => _mst;
            set
            {
                _mst = value;
                OnPropertyChanged(nameof(MST));
            }
        }

        private int _followingCount;
        public int FollowingCount
        {
            get => _followingCount;
            set
            {
                _followingCount = value;
                OnPropertyChanged(nameof(FollowingCount));
            }
        }

        private int _followerCount;
        public int FollowerCount
        {
            get => _followerCount;
            set
            {
                _followerCount = value;
                OnPropertyChanged(nameof(FollowerCount));
            }
        }

        public List<Guid> FollowingList { get; set; }
        public List<Guid> FollowerList { get; set; }

        public Command EmployeeRegisterCommand { get; set; }

        public AccountPageViewModel()
        {
            _client = BsdHttpClient.Instance();
            Company = new Company();

            User = new User()
            {
                Id = Guid.Parse(UserLogged.Id),
                Email = UserLogged.Email,
                Phone = UserLogged.Phone,
                Password = UserLogged.Password,
                FullName = UserLogged.FullName,
                Sex = (short)UserLogged.Sex,
                Birthday = UserLogged.Birthday,
                Address = UserLogged.Address,
                AvatarUrl = UserLogged.AvatarUrl,
                FacebookId = long.Parse(UserLogged.FacebookId),
                GoogleId = UserLogged.GoogleId,
                ZaloId = long.Parse(UserLogged.ZaloId),
                ProvinceId = UserLogged.ProvinceId,
                DistrictId = UserLogged.DistrictId,
                WardId = UserLogged.WardId,
                Street = UserLogged.Street
            };

            if (string.IsNullOrEmpty(UserLogged.CompanyId))
            {
                User.CompanyId = null;
            }
            else
            {
                User.CompanyId = Guid.Parse(UserLogged.CompanyId);
            }

            SocialCount = 0;
            if (User.FacebookId != -1)
                SocialCount++;
            if (User.ZaloId != -1)
                SocialCount++;
            if (!string.IsNullOrEmpty(User.GoogleId))
                SocialCount++;

            if (string.IsNullOrEmpty(User.CompanyId.ToString()))
            {
                Company.Name = " ";
            }
            else
            {
                //get company from api
                GetCompany(User.CompanyId.Value);
            }

            //lay thong tin dang theo doi - ng theo doi
            GetFollowings();
            GetFollowers();

            //cap nhat lai thong tin dang theo doi khi loginer follow/unfollow
            MessagingCenter.Subscribe<UserProfilePageViewModel, Guid>(this, "UpdateFollowing", async (sender, arg) =>
            {
                GetFollowings();
            });
        }


        private async void GetCompany(Guid id)
        {
            try
            {
                var response = await ApiHelper.Get<Company>($"api/company/{id}");
                if (response.IsSuccess)
                {
                    Company = (Company)response.Content;
                }
            }
            catch (Exception ex)
            {
                Company.Name = " ";
                await Application.Current.MainPage.DisplayAlert("Thông báo", ex.Message, Language.dong);
            }
        }

        private async void GetFollowings()
        {
            try
            {
                var response = await ApiHelper.Get<List<Guid>>($"api/following/following", true);
                if (response.IsSuccess)
                {
                    FollowingList = response.Content as List<Guid>;
                    FollowingCount = FollowingList.Count;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Thông báo", ex.Message, Language.dong);
            }
        }

        async void GetFollowers()
        {
            try
            {
                var response = await ApiHelper.Get<List<Guid>>($"api/following/follower", true);
                if (response.IsSuccess)
                {
                    FollowerList = response.Content as List<Guid>;
                    FollowerCount = FollowerList.Count;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Thông báo", ex.Message, Language.dong);
            }
        }
    }
}
