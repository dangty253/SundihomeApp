using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;

namespace SundihomeApp.ViewModels.GiaDatViewModels
{
    public class GiaDatHCMPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<Street> Streets { get; set; }
        public ObservableCollection<StreetDistance> StreetDistances { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }

        private Street _street;
        public Street Street { get => _street; set { this._street = value; OnPropertyChanged(nameof(Street)); } }

        private StreetDistance _streetDistance;
        public StreetDistance StreetDistance { get => _streetDistance; set { this._streetDistance = value; OnPropertyChanged(nameof(StreetDistance)); } }
        public GiaDatHCMPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            Streets = new ObservableCollection<Street>();
            StreetDistances = new ObservableCollection<StreetDistance>();
        }

        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/79", false, false);
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
            ApiResponse apiResponse = await ApiHelper.Get<List<Street>>($"{ApiRouter.GIADAT_STREET}/{this.District.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<Street> data = (List<Street>)apiResponse.Content;
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
            ApiResponse apiResponse = await ApiHelper.Get<List<StreetDistance>>($"{ApiRouter.GIADAT_STREETDISTANCE}/{this.Street.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<StreetDistance> data = (List<StreetDistance>)apiResponse.Content;
                foreach (var item in data)
                {
                    StreetDistances.Add(item);
                }
            }
        }
    }
}
