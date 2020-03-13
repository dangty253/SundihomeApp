using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class ContactDetailContentViewModel : BaseViewModel
    {
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } = new ObservableCollection<FloatButtonItem>();
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();
        public ObservableCollection<Option> GroupList { get; set; } = new ObservableCollection<Option>();
        public List<LoaiBatDongSanModel> LoaiBatDongSanList = new List<LoaiBatDongSanModel>();

        public ContactModel _contact;
        public ContactModel Contact
        {
            get => _contact;
            set
            {
                _contact = value;
                OnPropertyChanged(nameof(Contact));
            }
        }


        private ContactModel _contactModel;
        public ContactModel ContactModel
        {
            get => _contactModel;
            set
            {
                _contactModel = value;
                OnPropertyChanged(nameof(ContactModel));
            }
        }

        private Option _selectGroup;
        public Option SelectGroup
        {
            get => _selectGroup;
            set
            {
                _selectGroup = value;
                OnPropertyChanged(nameof(SelectGroup));
            }
        }
        public ContactDetailContentViewModel()
        {
            ContactModel = new ContactModel();
        }
        //get list provice
        public async Task GetProviceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            List<Province> data = (List<Province>)apiResponse.Content;
            foreach (var item in data)
            {
                ProvinceList.Add(item);
            }
        }

        //get list district
        public async Task GetDistrictAsync(int? id)
        {
            this.DistrictList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{id}", false, false);
            List<District> data = (List<District>)apiResponse.Content;
            foreach (var item in data)
            {
                DistrictList.Add(item);
            }
        }

        //get list ward
        public async Task GetWardAsync(int? id)
        {
            this.WardList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{id}", false, false);
            List<Ward> data = (List<Ward>)apiResponse.Content;
            foreach (var item in data)
            {
                WardList.Add(item);
            }
        }


    }
}
