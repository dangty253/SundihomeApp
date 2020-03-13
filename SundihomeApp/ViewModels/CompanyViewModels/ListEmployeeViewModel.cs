using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MongoDB.Driver;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class ListEmployeeViewModel :BaseViewModel
    {
        public ObservableCollection<User> UserList { get; set; } = new ObservableCollection<User>();
        public Guid _id;
        public bool DataNull = false;
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    _page = 1;
                    UserList.Clear();
                    DataNull = false;
                    await LoadUserOfCompany();
                    IsRefreshing = false;
                });
            }
        }

        private int _page;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }

        public ListEmployeeViewModel()
        {
            _page = 1;
        }
        public async Task LoadUserOfCompany()
        {
            ApiResponse response = await ApiHelper.Get<List<User>>($"api/company/GetUser/{_id}?page={Page}");
            List<User> data = (List<User>)response.Content;

            if (data.Count == 0)
            {
                DataNull = true;
                return;
            }
            foreach (var item in data)
            {
                UserList.Add(item);
            }

        }
    }
}
