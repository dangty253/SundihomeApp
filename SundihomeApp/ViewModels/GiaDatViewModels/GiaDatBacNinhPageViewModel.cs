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

    public class GiaDatBacNinhPageViewModel : BaseViewModel
    {
        public ObservableCollection<District> Districts { get; set; }
        public ObservableCollection<Ward> Wards { get; set; }
        public ObservableCollection<GiaDat_BacNinh_KhuDanCu> KhuDanCuList { get; set; }
        public ObservableCollection<GiaDat_BacNinh> GiaDatList { get; set; }
        public ObservableCollection<GiaDat_BacNinh_KhuVuc> KhuVucList { get; set; }

        private District _district;
        public District District { get => _district; set { this._district = value; OnPropertyChanged(nameof(District)); } }

        private District _ward;
        public District Ward { get => _ward; set { this._ward = value; OnPropertyChanged(nameof(Ward)); } }

        private bool _isNoCity;
        public bool IsNoCity { get => _isNoCity; set { this._isNoCity = value; OnPropertyChanged(nameof(IsNoCity)); } }

        private GiaDat_BacNinh_KhuDanCu _khuDanCu;
        public GiaDat_BacNinh_KhuDanCu KhuDanCu { get => _khuDanCu; set { this._khuDanCu = value; OnPropertyChanged(nameof(KhuDanCu)); } }

        private GiaDat_BacNinh_KhuDanCu _khuVuc;
        public GiaDat_BacNinh_KhuDanCu KhuVuc { get => _khuVuc; set { this._khuVuc = value; OnPropertyChanged(nameof(KhuVuc)); } }

        private GiaDat_BacNinh _giaDat;
        public GiaDat_BacNinh GiaDat
        {
            get => _giaDat;
            set
            {
                this._giaDat = value;
                OnPropertyChanged(nameof(GiaDat));
            }
        }

        public GiaDatBacNinhPageViewModel()
        {
            Districts = new ObservableCollection<District>();
            Wards = new ObservableCollection<Ward>();
            KhuDanCuList = new ObservableCollection<GiaDat_BacNinh_KhuDanCu>();
            GiaDatList = new ObservableCollection<GiaDat_BacNinh>();
            KhuVucList = new ObservableCollection<GiaDat_BacNinh_KhuVuc>();
            this.IsNoCity = true;
        }

        public async Task GetDistrictAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/27", false, false);
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

        public async Task LoadKhuDanCuList(bool isKhuDanCu)
        {
            this.KhuDanCuList.Clear();
            if (this.District == null || (this.Ward == null && this.IsNoCity)) return;
            string url;
            if (!IsNoCity && isKhuDanCu) url = $"{ApiRouter.GIADAT_BACNINH_KHUDANCU_DISTRICT}/{this.District.Id}";
            else if (!IsNoCity && !isKhuDanCu) url = $"{ApiRouter.GIADAT_BACNINH_STREETS_DISTRICT}/{this.District.Id}";
            else if (IsNoCity && isKhuDanCu) url = $"{ApiRouter.GIADAT_BACNINH_KHUDANCU_WARD}/{this.Ward.Id}";
            else url = $"{ApiRouter.GIADAT_BACNINH_STREETS_WARD}/{this.Ward.Id}";
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_BacNinh_KhuDanCu>>(url, false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_BacNinh_KhuDanCu> data = (List<GiaDat_BacNinh_KhuDanCu>)apiResponse.Content;
                foreach (var item in data)
                {
                    KhuDanCuList.Add(item);
                }
            }
        }
        public async Task LoadKhuVucList()
        {
            this.KhuVucList.Clear();
            if (this.Ward == null) return;
            ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_BacNinh_KhuDanCu>>($"{ApiRouter.GIADAT_BACNINH_KHUVUC_WARD}/{this.Ward.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<GiaDat_BacNinh_KhuVuc> data = (List<GiaDat_BacNinh_KhuVuc>)apiResponse.Content;
                foreach (var item in data)
                {
                    KhuVucList.Add(item);
                }
            }
        }
        public async Task LoaGiaDatList(bool isKhuDanCu)
        {
            try
            {
                this.GiaDatList.Clear();

                if (this.KhuDanCu == null) return;
                ApiResponse apiResponse = await ApiHelper.Get<List<GiaDat_BacNinh>>($"{(isKhuDanCu ? ApiRouter.GIADAT_BACNINH_KHUDANCU : ApiRouter.GIADAT_BACNINH_STREETS_DISTANCES)}/{this.KhuDanCu.Id}", false, false);
                if (apiResponse.IsSuccess)
                {
                    List<GiaDat_BacNinh> data = (List<GiaDat_BacNinh>)apiResponse.Content;
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
