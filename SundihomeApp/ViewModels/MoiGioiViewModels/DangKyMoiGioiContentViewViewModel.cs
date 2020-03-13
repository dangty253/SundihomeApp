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
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class DangKyMoiGioiContentViewViewModel: BaseViewModel
    {
        public ObservableCollection<Province> ProvinceList { get; set; }
        public ObservableCollection<District> DistrictList { get; set; }
        public List<int> Years { get; set; }
        public List<Option> TypeList { get; set; }

        private MoiGioiModel _moiGioiModel;
        public MoiGioiModel MoiGioiModel
        {
            get => _moiGioiModel;
            set
            {
                _moiGioiModel = value;
                OnPropertyChanged(nameof(MoiGioiModel));
            }
        }

        private MoiGioi _moiGioi;
        public MoiGioi MoiGioi
        {
            get => _moiGioi;
            set
            {
                _moiGioi = value;
                OnPropertyChanged(nameof(MoiGioi));
            }
        }

        public DangKyMoiGioiContentViewViewModel()
        {
            MoiGioiModel = new MoiGioiModel();
            MoiGioi = new MoiGioi();
            ProvinceList = new ObservableCollection<Province>();
            DistrictList = new ObservableCollection<District>();
            Years = new List<int>();

            for (int i = DateTime.Today.Year; i >=1990 ; i--)
            {
                Years.Add(i);
            }

            TypeList = LoaiMoiGioiData.GetOptions();
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

        public async Task GetDistrictAsync()
        {
            DistrictList.Clear();
            if (MoiGioiModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{MoiGioiModel.ProvinceId}", false, false);
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
