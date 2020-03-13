using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class AllListPageViewModel : ViewModels.ListViewPageViewModel2<MoiGioi>
    {
        public string Keyword { get; set; }

        public bool ShowClearFilterButton => this.Province != null || this.District != null || this.Type != null;
        public ObservableCollection<Province> ProvinceList { get; set; }
        public ObservableCollection<District> DistrictList { get; set; }
        public List<Option> TypeList { get; set; }

        private Province _province;
        public Province Province
        {
            get => _province;
            set
            {
                _province = value;
                OnPropertyChanged(nameof(Province));
                OnPropertyChanged(nameof(ShowClearFilterButton));
            }
        }
        private District _district;
        public District District
        {
            get => _district;
            set
            {
                _district = value;
                OnPropertyChanged(nameof(District));
                OnPropertyChanged(nameof(ShowClearFilterButton));
            }
        }
        private Option _type;
        public Option Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(ShowClearFilterButton));
            }
        }
        public AllListPageViewModel()
        {
            ProvinceList = new ObservableCollection<Province>();
            DistrictList = new ObservableCollection<District>();
            TypeList = LoaiMoiGioiData.GetOptions();
            TypeList.Insert(0, new Option() { Id = -1, Name = Language.tat_ca });
            PreLoadData = new Command(() =>
            {
                string url = $"{ApiRouter.MOIGIOI_GETALL}?page={this.Page}";


                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    // ApiUrl = $"{ApiRouter.MOIGIOI_GETALL}?page={this.Page}"; // ?provinceId=123&districtId=12312
                    url += $"&keyword={this.Keyword}";
                }

                if (Province != null)
                {
                    url += $"&provinceId={this.Province.Id}";
                }

                if (District != null)
                {
                    url += $"&districtId={this.District.Id}";
                }
                if (Type != null)
                {
                    url += $"&type={this.Type.Id}";
                }

                ApiUrl = url;
            });
        }
        public async Task GetProvinceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            List<Province> data = (List<Province>)apiResponse.Content;
            foreach (var item in data)
            {
                ProvinceList.Add(item);
            }
            ProvinceList.Insert(0, new Province() { Id = -1, Name = Language.tat_ca, Sort = -1 });
        }

        public async Task GetDistrictAsync()
        {
            this.DistrictList.Clear();
            if (Province != null)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{Province.Id}", false, false);
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    DistrictList.Add(item);
                }
                DistrictList.Insert(0, new District() { Id = -1, Name = Language.tat_ca, Pre = null, ProvinceId = -1 });
            }

        }
    }
}
