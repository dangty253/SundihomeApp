using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using Xamarin.Forms;
using System.Linq;
using SundihomeApp.Resources;

namespace SundihomeApp.ViewModels
{
    public class UserFollowPageViewModel : BaseViewModel
    {
        HttpClient _client;
        private List<Guid> _listId { get; set; }

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

        public ObservableCollection<User> FollowingUsers { get; set; }
        public ObservableCollection<User> FollowerUsers { get; set; }
        public ObservableCollection<User> Users { get; set; }

        //danh sach nguoi theo doi
        public UserFollowPageViewModel(List<Guid> listID)
        {
            _client = BsdHttpClient.Instance();
            _listId = listID;
            FollowerUsers = new ObservableCollection<User>();
            IsLoading = true;
            if (_listId != null)
            {
                GetFollowerUsers(_listId);
            }
            Users = FollowerUsers;
            IsLoading = false;
        }

        //ds dang theo doi
        public UserFollowPageViewModel(bool isFollowing, List<Guid> listID)
        {
            _client = BsdHttpClient.Instance();
            _listId = listID;
            FollowingUsers = new ObservableCollection<User>();
            IsLoading = true;
            if (_listId != null)
            {
                GetFollowingUsers(_listId);
            }
            MessagingCenter.Subscribe<UserProfilePageViewModel, Guid>(this, "UpdateFollowing", async (sender, arg) =>
            {
                User = FollowingUsers.SingleOrDefault(x => x.Id == arg);
                if (User != null)
                {
                    FollowingUsers.Remove(User);
                }
                else
                {
                    await GetUser(arg);
                    FollowingUsers.Add(User);
                }
            });
            Users = FollowingUsers;
            IsLoading = false;
        }

        //get user
        async Task GetUser(Guid id)
        {
            try
            {
                var response = await ApiHelper.Get<User>($"api/user/{id}",true);
                if (response.IsSuccess)
                {
                    User = (User)response.Content;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
            }
        }

        async void GetFollowingUsers(List<Guid> listId)
        {
            foreach (var id in listId)
            {
                await GetUser(id);
                FollowingUsers.Add(User);
            }
            IsLoading = false;
        }

        async void GetFollowerUsers(List<Guid> listId)
        {
            foreach (var id in listId)
            {
                await GetUser(id);
                FollowerUsers.Add(User);
            }
            IsLoading = false;
        }
    }
}
