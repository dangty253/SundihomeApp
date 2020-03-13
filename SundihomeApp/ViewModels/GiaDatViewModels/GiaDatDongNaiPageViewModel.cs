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
    public class GiaDatDongNaiPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<Ward> Wards { get; set; }
        public ObservableCollection<GiaDat_DongNai_Duong> Streets { get; set; }
        public ObservableCollection<GiaDat_DongNai_DoanDuong> StreetDistances { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }

        private Ward _ward;
        public Ward Ward { get => _ward; set { this._ward = value; OnPropertyChanged(nameof(Ward)); } }

        private GiaDat_DongNai_Duong _street;
        public GiaDat_DongNai_Duong Street { get => _street; set { this._street = value; OnPropertyChanged(nameof(Street)); } }

        private bool _isCity;
        public bool IsCity { get => _isCity; set { this._isCity = value; OnPropertyChanged(nameof(IsCity)); } }

        private GiaDat_DongNai_DoanDuong _streetDistance;
        public GiaDat_DongNai_DoanDuong StreetDistance { get => _streetDistance; set { this._streetDistance = value; OnPropertyChanged(nameof(StreetDistance)); } }
        public GiaDatDongNaiPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            Wards = new ObservableCollection<Ward>();
            Streets = new ObservableCollection<GiaDat_DongNai_Duong>();
            StreetDistances = new ObservableCollection<GiaDat_DongNai_DoanDuong>();
            IsCity = true;
        }

        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/75", false, false);
            if (apiResponse.IsSuccess)
            {
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    Districts.Add(item);
                }
            }
        }

        public async Task LoadWards()
        {
            this.Wards.Clear();
            if (this.District == null) return;
            ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{this.District.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<Ward> data = (List<Ward>)apiResponse.Content;
                foreach (var item in data)
                {
                    Wards.Add(item);
                }
            }
        }

        public async Task LoadStreets()
        {
            this.Streets.Clear();
            if (this.District == null) return;
            ApiResponse apiResponse;
            if (this.District.Id == 731 || this.District.Id==732)
            {
                apiResponse = await ApiHelper.Get<List<GiaDat_DongNai_Duong>>($"{ApiRouter.GIADAT_DONGNAI_STREETS_DISTRICT}/{this.District.Id}", false, false);
            }
            else
            {
                if (this.Ward == null) return;
                apiResponse = await ApiHelper.Get<List<GiaDat_DongNai_Duong>>($"{ApiRouter.GIADAT_DONGNAI_STREETS_WARD}/{this.Ward.Id}", false, false);
            }

            if (apiResponse.IsSuccess)
            {
                List<GiaDat_DongNai_Duong> data = (List<GiaDat_DongNai_Duong>)apiResponse.Content;
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
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_DongNai_DoanDuong>>($"{ApiRouter.GIADAT_DONGNAI_STREET_DISTANCES}/{this.Street.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_DongNai_DoanDuong> data = (List<GiaDat_DongNai_DoanDuong>)apiResponse.Content;
                foreach (var item in data)
                {
                    StreetDistances.Add(item);
                }
            }
        }
    }
}
