using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;

namespace SundihomeApp.ViewModels
{
    public class PostListTypePageViewModel : BaseViewModel
    {
        public ObservableCollection<Province> ProvinceList { get; set; }
        public ObservableCollection<District> DistrictList { get; set; }
        public List<LoaiBatDongSanModel> TypeList { get; set; }

        public bool ShowClearFilterButton => (this.Province != null || this.District != null || this.Type != null);

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
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowClearFilterButton));
            }
        }
        private LoaiBatDongSanModel _type;
        public LoaiBatDongSanModel Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowClearFilterButton));
            }
        }
        public PostListTypePageViewModel() 
        {
            ProvinceList = new ObservableCollection<Province>();
            DistrictList = new ObservableCollection<District>();
            TypeList = LoaiBatDongSanModel.GetList(null);
            TypeList.Insert(0, new LoaiBatDongSanModel() { Id = -1, Name = Language.tat_ca });
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
