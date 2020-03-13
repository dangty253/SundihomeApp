using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class CompanyDetailPageViewModel : BaseViewModel
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

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public CompanyDetailPageViewModel()
        {
            _page = 1;
            EmployeeModel = new EmployeeModel();
            InviteUser = new InviteUser();
        }
        private EmployeeModel _employeesModel;
        public EmployeeModel EmployeeModel
        {
            get => _employeesModel;
            set
            {
                _employeesModel = value;
                OnPropertyChanged(nameof(EmployeeModel));
            }
        }
        private InviteUser _inviteUser;
        public InviteUser InviteUser
        {
            get => _inviteUser;
            set
            {
                _inviteUser = value;
                OnPropertyChanged(nameof(InviteUser));
            }
        }

        public async Task LoadDetailCompany()
        {
            ApiResponse response = await ApiHelper.Get<Company>($"api/company/{_id}");
            if (response.IsSuccess)
            {
                this.Company = response.Content as Company;
            }
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

