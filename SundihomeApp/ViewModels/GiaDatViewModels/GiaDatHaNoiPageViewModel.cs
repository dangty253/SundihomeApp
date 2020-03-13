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
    public class GiaDatHaNoiPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<GiaDat_HaNoi_KhuDoThi> KhuDoThiList { get; set; }
        public ObservableCollection<GiaDat_HaNoi> GiaDatList { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }

        private GiaDat_HaNoi_KhuDoThi _khuDoThi;
        public GiaDat_HaNoi_KhuDoThi KhuDoThi { get => _khuDoThi; set { this._khuDoThi = value; OnPropertyChanged(nameof(KhuDoThi)); } }

        private GiaDat_HaNoi _giaDatHaNoi;
        public GiaDat_HaNoi GiaDatHaNoi { get => _giaDatHaNoi; set { this._giaDatHaNoi = value; OnPropertyChanged(nameof(GiaDatHaNoi)); } }

        private bool _showVT34;
        public bool ShowVT34
        {
            get => _showVT34;
            set
            {
                _showVT34 = value;
                OnPropertyChanged(nameof(ShowVT34));
            }
        }

        public GiaDatHaNoiPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            KhuDoThiList = new ObservableCollection<GiaDat_HaNoi_KhuDoThi>();
            GiaDatList = new ObservableCollection<GiaDat_HaNoi>();
        }

        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/1", false, false);
            if (apiResponse.IsSuccess)
            {
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    Districts.Add(item);
                }
            }
        }

        public async Task LoadKhuDoThiList(bool isKhuDoThi)
        {
            this.KhuDoThiList.Clear();
            if (this.District == null) return;
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_HaNoi_KhuDoThi>>($"{(isKhuDoThi?ApiRouter.GIADAT_HANOI_KHUDOTHI:ApiRouter.GIADAT_HANOI_STREETS)}/{this.District.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_HaNoi_KhuDoThi> data = (List<GiaDat_HaNoi_KhuDoThi>)apiResponse.Content;
                foreach (var item in data)
                {
                    KhuDoThiList.Add(item);
                }
            }
        }

        public async Task LoaGiaDatHaNoiList(bool isKhuDoThi)
        {
            try
            {
                this.GiaDatList.Clear();

                if (this.KhuDoThi == null) return;
                ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_HaNoi>>($"{(isKhuDoThi?ApiRouter.GIADAT_GIADATHANOI_MATCATDUONG: ApiRouter.GIADAT_GIADATHANOI_STREET_DISTANCES)}/{this.KhuDoThi.Id}", false, false);
                if (apiResponse.IsSuccess)
                {
                    List<GiaDat_HaNoi> data = (List<GiaDat_HaNoi>)apiResponse.Content;
                    foreach (var item in data)
                    {
                        GiaDatList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
