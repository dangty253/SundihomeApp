using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.GiaDatViewModels
{
    public class GiaDatHaNamPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<Ward> Wards { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }
        private Ward _ward;
        public Ward Ward { get => _ward; set { this._ward = value; OnPropertyChanged(nameof(Ward)); } }

        public GiaDatHaNamPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            Wards = new ObservableCollection<Ward>();
        }
        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/35", false, false);
            if (apiResponse.IsSuccess)
            {
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    Districts.Add(item);
                }
            }
        }
        public async Task GetWardAsync()
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
    }
}

