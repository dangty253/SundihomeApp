using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class CustomerListPageViewModel: BaseViewModel
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

        public ObservableCollection<User> Users { get; set; }

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

        private int _limit;
        public int Page { get; set; }

        public CustomerListPageViewModel()
        {
            _client = BsdHttpClient.Instance();
            _limit = 10;
            Page = 1;
            User = new User();
            Users = new ObservableCollection<User>();
            IsLoading = true;
        }

        public async void GetUsers()
        {
            try
            {
                var response = await ApiHelper.Get<List<User>>($"api/user?page={Page}&limit={_limit}");
                if (response.IsSuccess)
                {
                    List<User> users = response.Content as List<User>;
                    foreach (var user in users)
                    {
                        Users.Add(user);
                    }
                    this.IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("", ex.Message, Language.dong);
                this.IsLoading = false;
            }
        }
    }
}
