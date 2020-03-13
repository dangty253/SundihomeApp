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
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class PostPageViewModel : BaseViewModel
    {
        public IDictionary<int, string> StepList = new Dictionary<int, string>()
        {
            {1,Language.thong_tin_co_ban },
            {2,Language.thong_tin_chi_tiet },
            {3,Language.mo_ta_va_hinh_anh},
            {4,Language.thong_tin_bo_sung},
            {5,Language.luu },
        };

        public List<short> PhongTamList = new List<short>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<short> PhongNguList = new List<short>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public ObservableCollection<Project> ProjectList { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();
        public List<Option> DanhSachTienIchDuAn { get; set; }

        private List<int> _selectedUtitlitesId;
        public List<int> SelectedUtitlitesId { get => _selectedUtitlitesId; set { _selectedUtitlitesId = value; OnPropertyChanged(nameof(SelectedUtitlitesId)); } }

        public ObservableCollection<LoaiBatDongSanModel> LoaiBatDongSanList { get; set; }
        public ObservableCollection<TinhTrangPhapLyModel> TinhTrangPhaplyList { get; set; }
        public List<HuongModel> HuongList { get; set; }

        private PostModel _batDongSanModel;
        public PostModel PostModel
        {
            get => _batDongSanModel;
            set
            {
                _batDongSanModel = value;
                OnPropertyChanged(nameof(PostModel));
            }
        }

        private int _currentStep;
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged(nameof(CurrentStep));
                    OnPropertyChanged(nameof(NextTitle));
                }
            }
        }

        public string NextTitle
        {
            get
            {
                int nextStepIndex = _currentStep + 1;
                if (nextStepIndex > 0 && nextStepIndex < 6)
                {
                    return StepList[nextStepIndex].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        private bool _showProject;
        public bool ShowProject
        {
            get => _showProject;
            set
            {
                if (_showProject != value)
                {
                    _showProject = value;
                    OnPropertyChanged(nameof(ShowProject));
                }
            }
        }

        // show so tang cua can ho nay dang o, v
        private bool _showTang;
        public bool ShowTang
        {
            get => _showTang;
            set
            {
                if (_showTang != value)
                {
                    _showTang = value;
                    OnPropertyChanged(nameof(ShowTang));
                }
            }
        }

        // show so tang cua can ho nay co'.
        private bool _showSoTang;
        public bool ShowSoTang
        {
            get => _showSoTang;
            set
            {
                if (_showSoTang != value)
                {
                    _showSoTang = value;
                    OnPropertyChanged(nameof(ShowSoTang));
                }
            }
        }

        // hien thi chieu sau.
        private bool _showChieuSau;
        public bool ShowChieuSau
        {
            get => _showChieuSau;
            set
            {
                if (_showChieuSau != value)
                {
                    _showChieuSau = value;
                    OnPropertyChanged(nameof(ShowChieuSau));
                }
            }
        }

        private bool _showBathroom;
        public bool ShowBathroom
        {
            get => _showBathroom;
            set
            {
                if (_showBathroom != value)
                {
                    _showBathroom = value;
                    OnPropertyChanged(nameof(ShowBathroom));
                }
            }
        }

        private bool _showBedroom;
        public bool ShowBedroom
        {
            get => _showBedroom;
            set
            {
                if (_showBedroom != value)
                {
                    _showBedroom = value;
                    OnPropertyChanged(nameof(ShowBedroom));
                }
            }
        }

        private bool _showHuong;
        public bool ShowHuong
        {
            get => _showBathroom;
            set
            {
                if (_showHuong != value)
                {
                    _showHuong = value;
                    OnPropertyChanged(nameof(ShowHuong));
                }
            }
        }

        private bool _showUtilities;
        public bool Showutilities
        {
            get => _showUtilities;
            set
            {
                if (_showUtilities != value)
                {
                    _showUtilities = value;
                    OnPropertyChanged(nameof(Showutilities));
                }
            }
        }

        // cau hinh cho select multiple image and video.
        private IMultiMediaPickerService _multiMediaPickerService = null;
        public ObservableCollection<MediaFile> Media { get; set; } = new ObservableCollection<MediaFile>();
        public ICommand SelectImagesCommand { get; set; }

        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; }

        public PostPageViewModel()
        {
            // Init dữ liệu cố định
            LoaiBatDongSanList = new ObservableCollection<LoaiBatDongSanModel>();
            TinhTrangPhaplyList = new ObservableCollection<TinhTrangPhapLyModel>(TinhTrangPhapLyData.GetList());
            DanhSachTienIchDuAn = BDSUtilities.GetListUtilities();

            HuongList = HuongData.GetList();

            // Gan gia tri default.
            CurrentStep = 1;
            PostModel = new PostModel();

            SelectImagesCommand = new Command(SelectImages);

            ButtonCommandList = new ObservableCollection<FloatButtonItem>();
            _multiMediaPickerService = DependencyService.Get<IMediaPickerService>().GetMultiMediaPickerService();
            _multiMediaPickerService.OnMediaPicked += OnMediaPicked;
        }

        public Task LoadLoaiBatDongSan(int type)
        {
            LoaiBatDongSanList.Clear();
            var data = LoaiBatDongSanModel.GetList(type);
            for (int i = 0; i < data.Count; i++)
            {
                LoaiBatDongSanList.Add(data[i]);
            }
            return Task.CompletedTask;
        }

        public async Task GetProjects()
        {
            ProjectList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Project>>("api/project");
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
            if (PostModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{PostModel.ProvinceId}", false, false);
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
            if (PostModel.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{PostModel.DistrictId}", false, false);
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

        public void OnMediaPicked(object sender, MediaFile a)
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

        public async void SelectImages()
        {
            var action = await Shell.Current.DisplayActionSheet(Language.chon_hinh_anh, Language.huy, null, Language.thu_vien_anh, Language.chup_hinh);
            if (action == Language.thu_vien_anh)
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
