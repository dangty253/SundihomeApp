using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.CompanyEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class AddCompanyPageViewModel : BaseViewModel
    {
        public List<LoaiCongTyModel> ListLoaiCongTy { get; set; }
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        private IMultiMediaPickerService _multiMediaPickerService = null;
        public ObservableCollection<MediaFile> Media { get; set; } = new ObservableCollection<MediaFile>();
        public ICommand SelectImagesCommand { get; set; }
        public ICommand SelectLogoCommand { get; set; }

        public ObservableCollection<LichSuPhatTrienCongTy> LichSuPhatTrienCongTyList { get; set; } = new ObservableCollection<LichSuPhatTrienCongTy>();
        public int LichSuPhatTrienCongTyPage { get; set; } = 1;
        public ObservableCollection<ThanhTuuCongTy> ThanhTuuCongTyList { get; set; } = new ObservableCollection<ThanhTuuCongTy>();
        public int ThanhTuuCongTyPage { get; set; } = 1;
        public ObservableCollection<User> NhanVienUuTuCongTyList { get; set; } = new ObservableCollection<User>();

        public ObservableCollection<Option> ItemSourcePicker { get; set; }

       

        private string _titleCompany;
        public string TitleCompany
        {
            get => _titleCompany;
            set
            {
                _titleCompany = value;
                OnPropertyChanged(nameof(TitleCompany));
            }
        }
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private object _selectedYear;
        public object SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private bool _showMoreHistory;
        public bool ShowMoreHistory
        {
            get => _showMoreHistory;
            set
            {
                _showMoreHistory = value;
                OnPropertyChanged(nameof(ShowMoreHistory));
            }
        }

        private bool _showMoreSucceed;
        public bool ShowMoreSucceed
        {
            get => _showMoreSucceed;
            set
            {
                _showMoreSucceed = value;
                OnPropertyChanged(nameof(ShowMoreSucceed));
            }
        }

        private AddCompanyModel _addCompanyModel;
        public AddCompanyModel AddCompanyModel
        {
            get => _addCompanyModel;
            set
            {
                _addCompanyModel = value;
                OnPropertyChanged(nameof(AddCompanyModel));
            }
        }

        public AddCompanyPageViewModel()
        {
            ListLoaiCongTy = new List<LoaiCongTyModel>(LoaiCongTyData.GetListNganhNghe());
            AddCompanyModel = new AddCompanyModel();

            SelectLogoCommand = new Command(SelectLogo);
            SelectImagesCommand = new Command(SelectImages);
            GetMultiMediaPickerService();

            ItemSourcePicker = new ObservableCollection<Option>();
            int year = DateTime.Now.Year;
            int i = year + 1;
            while (year - i-- < 30)
            {
                var item = new Option();
                item.Id = i;
                item.Name = i.ToString();
                ItemSourcePicker.Add(item);
            }
        }


        //get list provice
        public async Task GetProviceAsync()
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

        //get list district
        public async Task GetDistrictAsync()
        {
            this.DistrictList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{AddCompanyModel.ProvinceId}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<District> data = (List<District>)apiResponse.Content;
                foreach (var item in data)
                {
                    DistrictList.Add(item);
                }
            }

        }

        //get list ward
        public async Task GetWardAsync()
        {
            this.WardList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{AddCompanyModel.DistrictId}", false, false);
            if (apiResponse.IsSuccess)
            {
                List<Ward> data = (List<Ward>)apiResponse.Content;
                foreach (var item in data)
                {
                    WardList.Add(item);
                }
            }
        }

        //get list Lich su phat trien cong ty
        public async Task GetLichSuPhatTrienCongTyAsync(Guid id)
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<LichSuPhatTrienCongTy>>(ApiRouter.COMPANY_GET_LICHSUPHATTRIENCONGTY + "/" + id + "?page=" + LichSuPhatTrienCongTyPage, true);
            if (apiResponse.IsSuccess)
            {
                List<LichSuPhatTrienCongTy> data = (List<LichSuPhatTrienCongTy>)apiResponse.Content;
                if (data.Count == 5) ShowMoreHistory = true;
                else ShowMoreHistory = false;

                foreach (var item in data)
                {
                    LichSuPhatTrienCongTyList.Add(item);
                }
            }
        }


        public async Task PostLichSuPhatTrienCongTyAsync()
        {
            LichSuPhatTrienCongTy item = new LichSuPhatTrienCongTy();
            item.CompanyId = Guid.Parse(UserLogged.CompanyId);
            item.Year = int.Parse(((Option)SelectedYear).Name);
            item.Description = Description;
            item.Id = Guid.NewGuid();
            item.Title = Title;

            ApiResponse apiResponse = await ApiHelper.Post(ApiRouter.COMPANY_POST_LICHSUPHATTRIENCONGTY, item);
            if (apiResponse.IsSuccess)
            {
                SelectedYear = null;
                Description = null;
                Title = null;
            }
        }
        public async Task DeleteLichSuPhatTrienCongTyAsync(LichSuPhatTrienCongTy item)
        {
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.COMPANY_DELETE_LICHSUPHATTRIENCONGTY + "/" + item.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                LichSuPhatTrienCongTyList.Remove(item);
            }
        }

        //get list thanh tuu phat trien cong ty
        public async Task GetThanhTuuCongTyAsync(Guid id)
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<ThanhTuuCongTy>>(ApiRouter.COMPANY_GET_THANHTUUCONGTY + "/" + id + "?page=" + ThanhTuuCongTyPage, true);
            if (apiResponse.IsSuccess)
            {
                List<ThanhTuuCongTy> data = (List<ThanhTuuCongTy>)apiResponse.Content;
                if (data.Count == 5) ShowMoreSucceed = true;
                else ShowMoreSucceed = false;

                foreach (var item in data)
                {
                    ThanhTuuCongTyList.Add(item);
                }
            }
        }


        public async Task PostThanhTuuCongTyAsync()
        {
            ThanhTuuCongTy item = new ThanhTuuCongTy();
            item.CompanyId = Guid.Parse(UserLogged.CompanyId);
            item.Year = int.Parse(((Option)SelectedYear).Name);
            item.Description = Description;
            item.Id = Guid.NewGuid();
            item.Title = Title;

            ApiResponse apiResponse = await ApiHelper.Post(ApiRouter.COMPANY_POST_THANHTUUCONGTY, item);
            if (apiResponse.IsSuccess)
            {
                SelectedYear = null;
                Description = null;
                Title = null;
            }
        }
        public async Task DeleteThanhTuuCongTyAsync(ThanhTuuCongTy item)
        {
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.COMPANY_DELETE_THANHTUUCONGTY + "/" + item.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                ThanhTuuCongTyList.Remove(item);
            }
        }
        //get list nhan vien uu tu
        public async Task GetNhanVienUuTuCongTyAsync(Guid id)
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<User>>(ApiRouter.COMPANY_GET_NHANVIENUUTUCONGTY + "/" + id, true);
            if (apiResponse.IsSuccess)
            {
                List<User> data = (List<User>)apiResponse.Content;

                for (int i = 0; i < 6 && i < data.Count; i++)
                {
                    NhanVienUuTuCongTyList.Add(data[i]);
                }
            }
        }
        public void CancelPopUp()
        {
            SelectedYear = null;
            Description = null;
            Title = null;
        }

        //chon logo
        async void SelectLogo()
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

                await TakeLogo();
            }
        }

        async Task TakeLogo()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Shell.Current.DisplayAlert(Language.quyen_truy_cap_bi_tu_choi, Language.khong_the_truy_cap_vao_thu_vien_anh, Language.dong);
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 300
            });

            if (file == null) return;
            var uri = new Uri($"{ApiConfig.CloudStorageApi}/api/files/upload?folder=company");
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            MultipartFormDataContent from = new MultipartFormDataContent();

            string logo = $"{Guid.NewGuid().ToString()}.jpg";
            var stream = new MemoryStream(File.ReadAllBytes(file.Path));
            var content = new StreamContent(stream);

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "files",
                FileName = logo
            };
            from.Add(content);

            try
            {
                HttpResponseMessage response = await client.PostAsync(uri, from);
                var res = JsonConvert.DeserializeObject<ApiResponse>(await response.Content.ReadAsStringAsync());

                if (!res.IsSuccess)
                {
                    await Shell.Current.DisplayAlert(Language.thong_bao, res.Message, Language.dong);
                    return;
                }
                AddCompanyModel.Logo = logo;
                AddCompanyModel.LogoFullUrl = ApiConfig.CloudStorageApiCDN + "/company/" + logo;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, ex.Message, Language.dong);
            }

        }


        //chon nhieu hinh
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

        //chon hinh anh
        async void SelectImages()
        {
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

                    await _multiMediaPickerService.PickPhotosAsync();
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

