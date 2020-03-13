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
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.IServices.ILiquidation;
using SundihomeApp.Models;
using SundihomeApp.Models.LiquidationModel;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.LiquidationViewModels
{
    public class AddLiquidationPageViewModel : BaseViewModel
    {
        private readonly ILiquidationCategoryService liquidationCategoryService;
        public List<LiquidationCategory> LiquidationCategories { get; set; }
        public ObservableCollection<Project> ProjectList { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        private AddLiquidationModel _liquidationModel;
        public AddLiquidationModel LiquidationModel
        {
            get => _liquidationModel;
            set
            {
                _liquidationModel = value;
                OnPropertyChanged(nameof(LiquidationModel));
            }
        }

        // cau hinh cho select multiple image and video.
        private IMultiMediaPickerService _multiMediaPickerService = null;
        public ObservableCollection<MediaFile> Media { get; set; } = new ObservableCollection<MediaFile>();
        public ICommand SelectImagesCommand { get; set; }

        public AddLiquidationPageViewModel()
        {
            SelectImagesCommand = new Command(SelectImages);
            _multiMediaPickerService = DependencyService.Get<IMediaPickerService>().GetMultiMediaPickerService();
            _multiMediaPickerService.OnMediaPicked += OnMediaPicked;

            liquidationCategoryService = DependencyService.Get<ILiquidationCategoryService>();
            LiquidationCategories = liquidationCategoryService.GetLiquidations();
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
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_hinh_anh, Language.sundihome_can_truy_cap_vao_thu_vien_de_lay_hinh_bds_cua_ban, Language.dong_y);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        photoPermisstionStatue = await PermissionHelper.CheckPermissions(pickPhotoStatus, Language.quyen_truy_cap_hinh_anh, Language.sundihome_can_truy_cap_vao_thu_vien_de_lay_hinh_bds_cua_ban);
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
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_may_anh, Language.sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_bds, Language.dong_y);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        cameraStatus = await PermissionHelper.CheckPermissions(Permission.Camera, Language.quyen_truy_cap_may_anh, Language.sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_bds);
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
            if (LiquidationModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{LiquidationModel.ProvinceId}", false, false);
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
            if (LiquidationModel.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{LiquidationModel.DistrictId}", false, false);
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
}
