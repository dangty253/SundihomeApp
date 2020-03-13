using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;

namespace SundihomeApp.ViewModels.Furniture
{
    public class FurnitureProductFiltePageViewModel : BaseViewModel
    {
        public ObservableCollection<FurnitureCategory> ParentCategories { get; set; }
        public ObservableCollection<FurnitureCategory> ChildCategories { get; set; }
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        private FurnitureCategory _parentCategory;
        public FurnitureCategory ParentCategory
        {
            get => _parentCategory;
            set
            {
                _parentCategory = value;
                OnPropertyChanged(nameof(ParentCategory));
            }
        }

        private FurnitureCategory _childCategory;
        public FurnitureCategory ChildCategory
        {
            get => _childCategory;
            set
            {
                _childCategory = value;
                OnPropertyChanged(nameof(ChildCategory));
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

        private string _keyword;
        public string Keyword
        {
            get => _keyword;
            set
            {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }

        public FurnitureProductFiltePageViewModel()
        {
            ParentCategories = new ObservableCollection<FurnitureCategory>();
            ChildCategories = new ObservableCollection<FurnitureCategory>();
        }

        public async Task LoadParentCategories()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<FurnitureCategory>>(Configuration.ApiRouter.FURNITURECATEGORY_GET_ONLY_PARENT);
            if (apiResponse.IsSuccess)
            {
                var categories = apiResponse.Content as List<FurnitureCategory>;
                int categoriesCount = categories.Count;
                for (int i = 0; i < categoriesCount; i++)
                {
                    var item = categories[i];
                    item.Name = Language.ResourceManager.GetString(item.LanguageKey, Language.Culture);
                    this.ParentCategories.Add(categories[i]);
                }
            }
        }

        public async Task LoadChildCategories()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<FurnitureCategory>>(Configuration.ApiRouter.FURNITURECATEGORY_GET_BY_PARENT + "/" + this.ParentCategory.Id);
            if (apiResponse.IsSuccess)
            {
                var categories = apiResponse.Content as List<FurnitureCategory>;
                int categoriesCount = categories.Count;
                for (int i = 0; i < categoriesCount; i++)
                {
                    var item = categories[i];
                    item.Name = Language.ResourceManager.GetString(item.LanguageKey, Language.Culture);
                    this.ChildCategories.Add(categories[i]);
                }
            }
        }


        public async Task GetProvinceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces");
            List<Province> data = (List<Province>)apiResponse.Content;
            foreach (var item in data)
            {
                ProvinceList.Add(item);
            }
        }

        public async Task GetDistrictAsync()
        {
            this.DistrictList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{this.Province.Id}", false, false);
            List<District> data = (List<District>)apiResponse.Content;
            foreach (var item in data)
            {
                DistrictList.Add(item);
            }
        }

        public async Task GetWardAsync()
        {
            WardList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{District.Id}", false, false);
            List<Ward> data = (List<Ward>)apiResponse.Content;
            foreach (var item in data)
            {
                WardList.Add(item);
            }
        }
    }
}
