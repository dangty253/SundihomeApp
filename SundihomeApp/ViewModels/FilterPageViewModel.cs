using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;

namespace SundihomeApp.ViewModels
{
    public class FilterPageViewModel : BaseViewModel
    {
        public ObservableCollection<Project> ProjectList { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        public List<LoaiBatDongSanModel> LoaiBatDongSanList { get; set; }
        public List<AreaFilterOtion> AreaOptions { get; set; }
        public List<PriceFilterOption> PriceOptions { get; set; }


        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                if (_project != value)
                {
                    _project = value;
                    OnPropertyChanged(nameof(Project));
                }
            }
        }

        private LoaiBatDongSanModel _loaiBatDongSan;
        public LoaiBatDongSanModel LoaiBatDongSan
        {
            get => _loaiBatDongSan;
            set
            {
                if (_loaiBatDongSan != value)
                {
                    _loaiBatDongSan = value;
                    OnPropertyChanged(nameof(LoaiBatDongSan));
                }
            }
        }

        private Province _province;
        public Province Province
        {
            get => _province;
            set
            {
                _province = value;
                OnPropertyChanged(nameof(Province));

                if (value != null)
                {
                    this.GetDistrictAsync();
                }
                else
                {
                    this.District = null;
                    this.DistrictList.Clear();
                }
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

                if (value != null)
                {
                    this.GetWardAsync();
                }
                else
                {
                    this.Ward = null;
                    this.WardList.Clear();
                }
            }
        }

        private Ward _ward;
        public Ward Ward
        {
            get => _ward;
            set
            {
                _ward = value;
                OnPropertyChanged(nameof(Ward));
            }
        }

        private AreaFilterOtion _area;
        public AreaFilterOtion Area
        {
            get => _area;
            set
            {
                _area = value;
                OnPropertyChanged(nameof(Area));
            }
        }

        private PriceFilterOption _priceFrom;
        public PriceFilterOption PriceFrom
        {
            get => _priceFrom;
            set
            {
                if (_priceFrom != value)
                {
                    _priceFrom = value;
                    OnPropertyChanged(nameof(PriceFrom));
                }
            }
        }

        private PriceFilterOption _priceTo;
        public PriceFilterOption PriceTo
        {
            get => _priceTo;
            set
            {
                if (_priceTo != value)
                {
                    _priceTo = value;
                    OnPropertyChanged(nameof(PriceTo));
                }
            }
        }


        private string _keyword;
        public string Keyword
        {
            get => _keyword;
            set
            {
                if (_keyword != value)
                {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                }
            }
        }

        private int? _soPhongTam;
        public int? SoPhongTam
        {
            get => _soPhongTam;
            set
            {
                if (_soPhongTam != value)
                {
                    _soPhongTam = value;
                    OnPropertyChanged(nameof(SoPhongTam));
                }
            }
        }

        private int? _soPhongNgu;
        public int? SoPhongNgu
        {
            get => _soPhongNgu;
            set
            {
                if (_soPhongNgu != value)
                {
                    _soPhongNgu = value;
                    OnPropertyChanged(nameof(SoPhongNgu));
                }
            }
        }

        public FilterPageViewModel()
        {
            this.AreaOptions = AreaFilterOtion.GetList();
            this.PriceOptions = PriceFilterOption.GetList();
            this.LoaiBatDongSanList = LoaiBatDongSanModel.GetList(null);

        }

        public async Task GetProjectsAsync()
        {
            ProjectList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Project>>("api/project/getall");
            if (apiResponse.IsSuccess)
            {
                List<Project> data = (List<Project>)apiResponse.Content;
                foreach (var item in data)
                {
                    ProjectList.Add(item);
                }
            }
        }

        public async Task GetProvinceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces");
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
            this.DistrictList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{this.Province.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    DistrictList.Add(item);
                }
            }
        }

        public async Task GetWardAsync()
        {
            WardList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{District.Id}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<Ward> data = (List<Ward>)apiResponse.Content;
                foreach (var item in data)
                {
                    WardList.Add(item);
                }
            }
        }
    }
}
