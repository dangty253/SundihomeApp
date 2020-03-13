using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class UserProfilePageViewModel : BaseViewModel
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

        private string _genderDisplay;
        public string GenderDisplay
        {
            get => _genderDisplay;
            set
            {
                _genderDisplay = value;
                OnPropertyChanged(nameof(GenderDisplay));
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

        private int _postTotal;
        public int PostTotal
        {
            get => _postTotal;
            set
            {
                _postTotal = value;
                OnPropertyChanged(nameof(PostTotal));
            }
        }

        private Post _post;
        public Post Post
        {
            get => _post;
            set
            {
                _post = value;
                OnPropertyChanged(nameof(Post));
            }
        }

        public ObservableCollection<Post> Posts { get; set; }

        private bool _isLoadMore;
        public bool IsLoadMore
        {
            get => _isLoadMore;
            set
            {
                _isLoadMore = value;
                OnPropertyChanged(nameof(IsLoadMore));
            }
        }

        public int Page { get; set; }

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

        private bool _isFollow;
        public bool IsFollow
        {
            get => _isFollow;
            set
            {
                _isFollow = value;
                OnPropertyChanged(nameof(IsFollow));
            }
        }

        private bool _isUnFollow;
        public bool IsUnFollow
        {
            get => _isUnFollow;
            set
            {
                _isUnFollow = value;
                OnPropertyChanged(nameof(IsUnFollow));
            }
        }

        public UserProfilePageViewModel()
        {
            IsLoading = true;
            Company = new Company() { Name = " " };
            User = new User();
            Post = new Post();
            Posts = new ObservableCollection<Post>();

            Page = 1;


        }


        // lay thong tin user
        public async Task GetUser(Guid id)
        {
            var response = await ApiHelper.Get<User>($"api/user/{id}");
            if (response.IsSuccess)
            {
                User = (User)response.Content;
                switch (User.Sex)
                {
                    case 0:
                        GenderDisplay = Language.nam;
                        break;
                    case 1:
                        GenderDisplay = Language.nu;
                        break;
                    case 2:
                        GenderDisplay = Language.khac;
                        break;
                }
                if (User.CompanyId.HasValue)
                {
                    await GetCompany(User.CompanyId.Value);
                }
            }
            IsLoading = false;
        }

        //lay thong tin cong ty cua user
        async Task GetCompany(Guid companyId)
        {
            ApiResponse apiResponse = await ApiHelper.Get<Company>($"api/company/{companyId}", true);
            if (apiResponse.IsSuccess)
            {
                Company = (Company)apiResponse.Content;
            }
        }

        //dem so bai post cua user
        public async Task GetPostTotal(Guid userId)
        {
            try
            {
                var response = await ApiHelper.Get<List<Post>>($"api/post/getbyuser/{userId}/all");
                if (response.IsSuccess)
                {
                    List<Post> posts = response.Content as List<Post>;
                    PostTotal = posts.Count;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Thông báo", ex.Message, Language.dong);
            }
        }

        //lay ds bai post cua user
        public async Task GetPosts(Guid userId)
        {
            int limit = 5;

            try
            {
                var response = await ApiHelper.Get<List<Post>>($"api/post/getbyuser/{userId}?limit={limit}&page={Page}");
                if (response.IsSuccess)
                {
                    List<Post> posts = response.Content as List<Post>;
                    if (posts.Count > 0)
                    {
                        foreach (var post in posts)
                        {
                            Posts.Add(post);
                        }
                        if (posts.Count < limit || (Page == (int)PostTotal / limit && PostTotal % limit == 0))
                        {
                            IsLoadMore = false;
                        }
                        else
                        {
                            IsLoadMore = true;
                        }
                    }
                    else
                    {
                        IsLoadMore = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Thông báo", ex.Message, Language.dong);
            }
        }

        //lay ds nguoi theo doi user
        public async Task GetFollowers(Guid userId)
        {
            var response = await ApiHelper.Get<List<Guid>>($"api/following/follower/{userId}");
            bool isFollow = false;
            if (response.IsSuccess)
            {
                if (response.Content != null)
                {
                    List<Guid> listId = response.Content as List<Guid>;
                    FollowerCount = listId.Count;
                    if (UserLogged.IsLogged && listId.Contains(Guid.Parse(UserLogged.Id)))
                    {
                        isFollow = true;
                    }
                }
                else
                    FollowerCount = 0;
            }
            IsFollow = isFollow;
            IsUnFollow = !IsFollow;
        }

        public async void Follow(Guid userId)
        {
            if (!UserLogged.IsLogged)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }

            IsLoading = true;
            ApiResponse response = await ApiHelper.Put($"api/following/follow/{userId}", null, true);
            if (response.IsSuccess)
            {
                await GetFollowers(userId);
                MessagingCenter.Send<UserProfilePageViewModel, Guid>(this, "UpdateFollowing", userId);
            }
            IsLoading = false;
        }
    }
}
