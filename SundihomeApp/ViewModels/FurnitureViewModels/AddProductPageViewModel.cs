using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Models.Furniture;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class AddProductPageViewModel : BaseViewModel
    {
        public ObservableCollection<FurnitureCategory> ParentCategories { get; set; }
        public ObservableCollection<FurnitureCategory> ChildCategories { get; set; }

        public ObservableCollection<Province> ProvinceList { get; set; } 
        public ObservableCollection<District> DistrictList { get; set; } 
        public ObservableCollection<Ward> WardList { get; set; }

        private AddProductModel _addProductModel;
        public AddProductModel AddProductModel
        {
            get => _addProductModel;
            set
            {
                _addProductModel = value;
                OnPropertyChanged(nameof(AddProductModel));
            }
        }

        private DateTime _dateNow;
        public DateTime DateNow
        {
            get => _dateNow;
            set
            {
                _dateNow = value;
                OnPropertyChanged(nameof(DateNow));
            }
        }

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

        private Company _company;
        public Company Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged(nameof(Company));
            }
        }

        private bool _switch;
        public bool Switch
        {
            get => _switch;
            set
            {
                _switch = value;
                OnPropertyChanged(nameof(Switch));
            }
        }

        private FurnitureProduct _product;
        public FurnitureProduct Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        // cau hinh cho select multiple image and video.
        private IMultiMediaPickerService _multiMediaPickerService = null;
        public ObservableCollection<MediaFile> Media { get; set; } = new ObservableCollection<MediaFile>();
        public List<string> ImageUrlToDelete { get; set; } = new List<string>();// list hinh anh can xoa. 
        public ICommand SelectImagesCommand { get; set; }
        public ICommand SelectVideosCommand { get; set; }

        public AddProductPageViewModel()
        {
            Product = new FurnitureProduct();

            ParentCategories = new ObservableCollection<FurnitureCategory>();
            ChildCategories = new ObservableCollection<FurnitureCategory>();

            Company = new Company();

            ProvinceList = new ObservableCollection<Province>();
            DistrictList = new ObservableCollection<District>();
            WardList = new ObservableCollection<Ward>();

            GetMultiMediaPickerService();
            SelectImagesCommand = new Command(SelectImages);
        }

        public async Task GetProduct(Guid id)
        {
            var response = await ApiHelper.Get<FurnitureProduct>($"{ApiRouter.FURNITUREPRODUCT_GET_BY_ID}{id}");
            if (!response.IsSuccess)
                return;
            Product = response.Content as FurnitureProduct;
        }

        public async Task GetCompany(Guid companyId)
        {
            ApiResponse apiResponse = await ApiHelper.Get<Company>($"api/company/{companyId}", true);
            if (apiResponse.IsSuccess)
            {
                Company = (Company)apiResponse.Content;
            }
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
                    this.ParentCategories.Add(item);
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
            this.DistrictList.Clear();
            if (AddProductModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{AddProductModel.ProvinceId}", false, false);
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

        public async Task GetWardAsync()
        {
            WardList.Clear();
            if (AddProductModel.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{AddProductModel.DistrictId}", false, false);
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

        public async void GetMultiMediaPickerService()
        {
            if (_multiMediaPickerService != null) return;
            await CrossMedia.Current.Initialize();
            _multiMediaPickerService = DependencyService.Get<IMediaPickerService>().GetMultiMediaPickerService();
            _multiMediaPickerService.OnMediaPicked += OnMediaPicked;
        }

        void OnMediaPicked(object sender, MediaFile a)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!Media.Any(x => x.PreviewPath == a.PreviewPath))
                {
                    // is uploaded = false;
                    Media.Add(a);
                }
            });
        }

        async void SelectImages()
        {
            var action = await Shell.Current.DisplayActionSheet(Language.chon_hinh_anh, Language.huy, null, Language.thu_vien, Language.chup_hinh);
            if (action == Language.thu_vien)
            {
                Permission pickPhotoStatus = Permission.Storage;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    pickPhotoStatus = Permission.Photos;
                }

                PermissionStatus photoPermisstionStatue = await CrossPermissions.Current.CheckPermissionStatusAsync(pickPhotoStatus);

                if (photoPermisstionStatue != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(pickPhotoStatus))
                    {
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_hinh_anh, Language.Sundihome_can_truy_cap_vao_thu_vien_hinh_anh_de_lay_hinh_san_pham_cua_ban, Language.dong_y);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        photoPermisstionStatue = await PermissionHelper.CheckPermissions(pickPhotoStatus, Language.quyen_truy_cap_hinh_anh, Language.Sundihome_can_truy_cap_vao_thu_vien_hinh_anh_de_lay_hinh_san_pham_cua_ban);
                    }
                    else
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(pickPhotoStatus);
                        photoPermisstionStatue = results[pickPhotoStatus];
                    }
                }
                if (photoPermisstionStatue == PermissionStatus.Granted)
                {
                    await _multiMediaPickerService.PickPhotosAsync();
                }
            }
            else if (action == Language.chup_hinh)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Shell.Current.DisplayAlert(Language.may_anh, Language.may_anh_khong_kha_dung, Language.dong);
                    return;
                }

                Plugin.Media.Abstractions.MediaFile file = null;
                PermissionStatus cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

                if (cameraStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_may_anh, Language.Sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_san_pham, Language.dong_y);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        cameraStatus = await PermissionHelper.CheckPermissions(Permission.Camera, Language.quyen_truy_cap_may_anh, Language.Sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_san_pham);
                    }
                    else
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        cameraStatus = results[Permission.Camera];
                    }
                }
                if (cameraStatus == PermissionStatus.Granted)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        SaveToAlbum = false,
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1000,
                    });
                }
                if (file != null)
                {
                    this.Media.Add(new MediaFile()
                    {
                        Type = MediaFileType.Image,
                        PreviewPath = file.Path,
                        Path = file.Path
                    });
                }
            }
        }

    }
}
