using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.BankViewModel
{
    public class GoiVayFilterresultViewModel : ListViewPageViewModel2<GoiVay>
    {
        public GoiVayFilterModel _filterModel;

        public bool ShowClearFilterButton => this.Province != null || this.District != null || this.Bank != null;

        public ObservableCollection<Bank> BankList { get; set; } = new ObservableCollection<Bank>();
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();

        private Bank _bank;
        public Bank Bank { get => _bank; set { _bank = value; OnPropertyChanged(nameof(Bank)); OnPropertyChanged(nameof(ShowClearFilterButton)); } }

        private Province _province;
        public Province Province { get => _province; set { _province = value; OnPropertyChanged(nameof(Province)); OnPropertyChanged(nameof(ShowClearFilterButton)); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); OnPropertyChanged(nameof(ShowClearFilterButton)); } }


        public GoiVayFilterresultViewModel(GoiVayFilterModel FilterModel = null)
        {
            if (FilterModel == null)
            {
                _filterModel = new GoiVayFilterModel();
            }
            else
            {
                _filterModel = FilterModel;
            }

            PreLoadData = new Command(() =>
            {
                var json = JsonConvert.SerializeObject(this._filterModel);
                ApiUrl = $"api/bank/goivay/filter?json={json}&page={this.Page}";
            });
        }


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
                ProvinceList.Insert(0, new Province() { Id = -1, Name = Language.tat_ca, Sort = -1 });
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
                    DistrictList.Insert(0, new District() { Id = -1, Name = Language.tat_ca, Pre = null, ProvinceId = -1 });
                }
            }
        }
        public async Task LoadBanksAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<Bank>>(ApiRouter.BANK_GETALL, false, false);
            if (apiResponse.IsSuccess)
            {
                List<Bank> data = (List<Bank>)apiResponse.Content;
                BankList.Add(new Bank() { Id = -1, Name = Language.tat_ca });
                foreach (var item in data)
                {
                    BankList.Add(item);
                }
            }
        }

    }
}
