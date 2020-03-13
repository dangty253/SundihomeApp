using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class ContentViewModalDiaryViewModel : BaseViewModel
    {
        private IMultiMediaPickerService _multiMediaDiaryPickerService = null;
        public ObservableCollection<MediaFile> MediaDiary { get; set; } = new ObservableCollection<MediaFile>();
        public ICommand SelectedImageDiaryCommand { get; set; }
        public ContentViewModalDiaryViewModel()
        {
            ProjectDiary = new ProjectDiary();
            SelectedImageDiaryCommand = new Command(SelectImageDiary);
            
        }
        private ProjectDiary _projectDiary;
        public ProjectDiary ProjectDiary
        {
            get => _projectDiary;
            set
            {
                _projectDiary = value;
                OnPropertyChanged(nameof(ProjectDiary));
            }
        }
        public async void GetMultiMediaDiaryPickerService()
        {
            if (_multiMediaDiaryPickerService != null) return;
            await CrossMedia.Current.Initialize();
            _multiMediaDiaryPickerService = DependencyService.Get<IMediaPickerService>().GetMultiMediaPickerService();
            _multiMediaDiaryPickerService.OnMediaPicked += OnMediaDiaryPicked;
        }
        void OnMediaDiaryPicked(object sender, MediaFile a)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!MediaDiary.Any(x => x.PreviewPath == a.PreviewPath))
                {
                    // is uploaded = false;
                    MediaDiary.Add(a);
                }
            });
        }

        public async void SelectImageDiary()
        {
            GetMultiMediaDiaryPickerService();
            var action = await Shell.Current.DisplayActionSheet(Language.chon_hinh_anh, Language.huy, null, Language.thu_vien, Language.may_anh);
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

                    await _multiMediaDiaryPickerService.PickPhotosAsync();
                }
            }
            else if (action == Language.may_anh)
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
                    this.MediaDiary.Add(new MediaFile()
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

