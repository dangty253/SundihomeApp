using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class AddProjectPageViewModel : BaseViewModel
    {
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        public ObservableCollection<ProjectDiary> ProjectDiaryList { get; set; } = new ObservableCollection<ProjectDiary>();

        public ObservableCollection<ProjectStatusModel> StatusList { get; set; }


        public List<Option> listLoaiDuAnModel { get; set; }
        private List<int> _loaiDuAnSelecteIds;
        public List<int> LoaiDuAnSelecteIds
        {
            get => _loaiDuAnSelecteIds;
            set
            {
                _loaiDuAnSelecteIds = value;
                OnPropertyChanged(nameof(LoaiDuAnSelecteIds));
            }
        }

        public List<Option> DanhSachTienIchDuAn { get; set; }

        private List<int> _tienIchDuAnSelecteIds;
        public List<int> TienIchDuAnSelecteIds
        {
            get => _tienIchDuAnSelecteIds;
            set
            {
                _tienIchDuAnSelecteIds = value;
                OnPropertyChanged(nameof(TienIchDuAnSelecteIds));
            }
        }

        private IMultiMediaPickerService _multiMediaPickerService = null;
        public ObservableCollection<MediaFile> Media { get; set; } = new ObservableCollection<MediaFile>();
        public ICommand SelectImagesCommand { get; set; }
        public ICommand SelectVideosCommand { get; set; }


        public AddProjectPageViewModel()
        {
            listLoaiDuAnModel = ProjectTypeData.GetListProjectType().Select(x => new Option() //
            {
                Id = x.Id,
                Name = x.Name,
                IsSelected = false
            }).ToList();
            Page = 1;

            DanhSachTienIchDuAn = BDSUtilities.GetListUtilities();


            AddProjectModel = new AddProjectModel();

            StatusList = new ObservableCollection<ProjectStatusModel>(ProjectStatusData.GetList());

            //GetMultiMediaPickerService();
            SelectImagesCommand = new Command(SelectImages);
        }

        private string _titlePostProject;
        public string TitlePostProject
        {
            get => _titlePostProject;
            set
            {
                _titlePostProject = value;
                OnPropertyChanged(nameof(TitlePostProject));
            }
        }

        private AddProjectModel _addProjectModel;
        public AddProjectModel AddProjectModel
        {
            get => _addProjectModel;
            set
            {
                _addProjectModel = value;
                OnPropertyChanged(nameof(AddProjectModel));
            }
        }


        private int _numUtilitiChecked;
        public int NumUtilitiChecked
        {
            get => _numUtilitiChecked;
            set
            {
                _numUtilitiChecked = value;
                OnPropertyChanged(nameof(NumUtilitiChecked));
            }
        }

        private int _page;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }
        private bool _showMoreDiary;
        public bool ShowMoreDiary
        {
            get => _showMoreDiary;
            set
            {
                _showMoreDiary = value;
                OnPropertyChanged(nameof(ShowMoreDiary));
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
            if (AddProjectModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{AddProjectModel.ProvinceId}", false, false);
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
            this.WardList.Clear();
            if (AddProjectModel.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{AddProjectModel.DistrictId}", false, false);
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

        public async Task GetProjectDiary(Guid Id)
        {
            ProjectDiaryList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<ProjectDiary>>($"{Configuration.ApiRouter.PROJECT_DIARY_GET_LIST_PROJECTDIARY_BYPROJECTID}/{Id}?page={Page}", true);
            if (apiResponse.IsSuccess)
            {
                List<ProjectDiary> data = (List<ProjectDiary>)apiResponse.Content;
                if (data.Count == 10) ShowMoreDiary = true;
                else ShowMoreDiary = false;
                foreach (var item in data)
                {
                    ProjectDiaryList.Add(item);
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
            GetMultiMediaPickerService();
            var action = await Shell.Current.DisplayActionSheet(Language.chon_hinh_anh, "", null, Language.thu_vien, Language.chup_hinh);
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
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_thu_vien, Language.sundihome_can_truy_cap_vao_thu_vien_de_lay_hinh_bds_cua_ban, Language.dong_y);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        photoPermisstionStatue = await PermissionHelper.CheckPermissions(pickPhotoStatus, Language.quyen_truy_cap_thu_vien, Language.sundihome_can_truy_cap_vao_thu_vien_de_lay_hinh_bds_cua_ban);
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

    }
}

