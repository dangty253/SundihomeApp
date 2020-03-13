using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class AddEmployeePageViewModel : BaseViewModel
    {
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();
        public AddEmployeePageViewModel()
        {
            EmployeeModel = new EmployeeModel();
            InviteUser = new InviteUser();
        }

        public async Task GetProvinceAsync()
        {
            this.ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            List<Province> data = (List<Province>)apiResponse.Content;
            foreach (var item in data)
            {
                ProvinceList.Add(item);
            }
        }

        public async Task GetDistrictAsync()
        {
            this.DistrictList.Clear();
            if (EmployeeModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{EmployeeModel.ProvinceId}", false, false);
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    DistrictList.Add(item);
                }
            }
        }

        public async Task GetWardAsync()
        {
            this.WardList.Clear();
            if (EmployeeModel.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{EmployeeModel.DistrictId}", false, false);
                List<Ward> data = (List<Ward>)apiResponse.Content;
                foreach (var item in data)
                {
                    WardList.Add(item);
                }
            }
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
    }
}

