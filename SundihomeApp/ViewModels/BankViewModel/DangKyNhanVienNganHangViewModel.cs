using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;

namespace SundihomeApp.ViewModels.BankViewModel
{
    public class DangKyNhanVienNganHangViewModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public ObservableCollection<Bank> BankList { get; set; }
        public ObservableCollection<Province> ProvinceList { get; set; }
        public ObservableCollection<District> DistrictList { get; set; }

        private Bank _bank;
        public Bank Bank { get => _bank; set { _bank = value; OnPropertyChanged(nameof(Bank)); } }

        private Province _province;
        public Province Province { get => _province; set { _province = value; OnPropertyChanged(nameof(Province)); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); } }

        public DangKyNhanVienNganHangViewModel()
        {
            BankList = new ObservableCollection<Bank>();
            ProvinceList = new ObservableCollection<Province>();
            DistrictList = new ObservableCollection<District>();
        }

        public async Task GetProvinceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            if (apiResponse.IsSuccess)
            {
                List<Province> data = (List<Province>)apiResponse.Content;
                foreach (var item in data)
                {
                    ProvinceList.Add(item);
                }
            }
        }

        public async Task LoadBanks()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<Bank>>(ApiRouter.BANK_GETALL, false, false);
            if (apiResponse.IsSuccess)
            {
                List<Bank> data = (List<Bank>)apiResponse.Content;
                foreach (var item in data)
                {
                    BankList.Add(item);
                }
            }
        }

        public async Task GetDistrictAsync()
        {
            DistrictList.Clear();
            District = null;
            if (Province != null)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{Province.Id}", false, false);
                if (apiResponse.IsSuccess)
                {
                    List<District> data = (List<District>)apiResponse.Content;
                    foreach (var item in data)
                    {
                        DistrictList.Add(item);
                    }
                }
            }
        }
    }
}
