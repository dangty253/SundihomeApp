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
    public class GiaDatHaTinhPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<GiaDat_HaTinh_Duong> Streets { get; set; }
        public ObservableCollection<GiaDat_HaTinh_DoanDuong> StreetDistances { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }

        
        private GiaDat_HaTinh_Duong _street;
        public GiaDat_HaTinh_Duong Street { get => _street; set { this._street = value; OnPropertyChanged(nameof(Street)); } }

        
        private GiaDat_HaTinh_DoanDuong _streetDistance;
        public GiaDat_HaTinh_DoanDuong StreetDistance { get => _streetDistance; set { this._streetDistance = value; OnPropertyChanged(nameof(StreetDistance)); } }
        public GiaDatHaTinhPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            Streets = new ObservableCollection<GiaDat_HaTinh_Duong>();
            StreetDistances = new ObservableCollection<GiaDat_HaTinh_DoanDuong>();
        }

        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/42", false, false);
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
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_HaTinh_Duong>>($"{ApiRouter.GIADAT_HATINH_STREETS}/{this.District.Id}", false, false);

            if (apiResponse.IsSuccess)
            {
                List<GiaDat_HaTinh_Duong> data = (List<GiaDat_HaTinh_Duong>)apiResponse.Content;
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
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_HaTinh_DoanDuong>>($"{ApiRouter.GIADAT_HATINH_STREET_DISTANCES}/{this.Street.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_HaTinh_DoanDuong> data = (List<GiaDat_HaTinh_DoanDuong>)apiResponse.Content;
                foreach (var item in data)
                {
                    StreetDistances.Add(item);
                }
            }
        }
    }
}
