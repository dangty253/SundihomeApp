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
    public class FilterPageViewModel : BaseViewModel
    {
        public ObservableCollection<Bank> BankList { get; set; } = new ObservableCollection<Bank>();
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();

        private Bank _bank;
        public Bank Bank { get => _bank; set { _bank = value; OnPropertyChanged(nameof(Bank)); } }

        private Province _province;
        public Province Province { get => _province; set { _province = value; OnPropertyChanged(nameof(Province)); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); } }

        public async Task LoadProvinceAsync()
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

        public async Task LoadBanksAsync()
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

        public async Task LoadDistrictAsync()
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
