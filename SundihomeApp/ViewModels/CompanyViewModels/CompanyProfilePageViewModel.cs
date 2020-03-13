using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stormlion.PhotoBrowser;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class CompanyProfilePageViewModel : BaseViewModel
    {
        public Guid _id;
        public ObservableCollection<FurnitureProduct> ListProducts { get; set; } = new ObservableCollection<FurnitureProduct>();
        public bool DataNull = false;
        public bool booleanTappedProduct = false;
        public bool booleanTappedEployee = false;
        public bool booleanTappedCompany = false;
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private int _page;
        public int Page
        {
            get => _page;
            set
            {
                if (_page != value)
                {
                    _page = value;
                    OnPropertyChanged(nameof(Page));
                }
            }
        }
        public ICommand RefreshCommand
        {
            get => new Command(LoadOnRefreshCommand);
        }
        
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
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

        public CompanyProfilePageViewModel()
        {
            _page = 1;
            EmployeeModel = new EmployeeModel();
            InviteUser = new InviteUser();
        }
        public async Task LoadDetailCompany()
        {
            ApiResponse response = await ApiHelper.Get<Company>($"api/company/{_id}");
            if (response.IsSuccess)
            {
                this.Company = response.Content as Company;
            }
        }
        public async Task loadProducts()
        {
            ApiResponse response = await ApiHelper.Get<List<FurnitureProduct>>($"api/furnitureproduct/company/{_id}?page={Page}");
            List<FurnitureProduct> listProduct = response.Content as List<FurnitureProduct>;
            if (listProduct.Count == 0)
            {
                DataNull = true;
                return;
            }
            foreach (var item in listProduct)
            {
                ListProducts.Add(item);
            }
        }

        public async void LoadOnRefreshCommand()
        {
            IsRefreshing = true;
            _page = 1;
            ListProducts.Clear();
            DataNull = false;
            await loadProducts();
            IsRefreshing = false;
        }
    }
}

