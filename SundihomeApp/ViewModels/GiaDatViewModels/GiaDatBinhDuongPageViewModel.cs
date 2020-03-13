using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.GiaDat;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;

namespace SundihomeApp.ViewModels.GiaDatViewModels
{
    public class GiaDatBinhDuongPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<GiaDat_BinhDuong_Duong> Streets { get; set; }
        public ObservableCollection<GiaDat_BinhDuong_DoanDuong> StreetDistances { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }

        private GiaDat_BinhDuong_Duong _street;
        public GiaDat_BinhDuong_Duong Street { get => _street; set { this._street = value; OnPropertyChanged(nameof(Street)); } }

        private GiaDat_BinhDuong_DoanDuong _streetDistance;
        public GiaDat_BinhDuong_DoanDuong StreetDistance { get => _streetDistance; set { this._streetDistance = value; OnPropertyChanged(nameof(StreetDistance)); } }
        public GiaDatBinhDuongPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            Streets = new ObservableCollection<GiaDat_BinhDuong_Duong>();
            StreetDistances = new ObservableCollection<GiaDat_BinhDuong_DoanDuong>();
        }

        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/74", false, false);
            if (apiResponse.IsSuccess)
            {
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    Districts.Add(item);
                }
            }
        }

        public async Task LoadStreets()
        {
            this.Streets.Clear();
            if (this.District == null) return;
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_BinhDuong_Duong>>($"{ApiRouter.GIADAT_BINHDUONG_STREET}/{this.District.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_BinhDuong_Duong> data = (List<GiaDat_BinhDuong_Duong>)apiResponse.Content;
                foreach (var item in data)
                {
                    Streets.Add(item);
                }
            }
        }


        public async Task LoadStreetDistances()
        {
            this.StreetDistances.Clear();
            if (this.Street == null) return;
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_BinhDuong_DoanDuong>>($"{ApiRouter.GIADAT_BINHDUONG_STREET_DISTANCES}/{this.Street.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_BinhDuong_DoanDuong> data = (List<GiaDat_BinhDuong_DoanDuong>)apiResponse.Content;
                foreach (var item in data)
                {
                    StreetDistances.Add(item);
                }
            }
        }
    }
}
