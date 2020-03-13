using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.BankViewModel;
using Xamarin.Forms;

namespace SundihomeApp.Views.BankViews
{
    public partial class AddLoanView : ContentView
    {
        public event EventHandler OnSaved;
        public event EventHandler OnCancel;
        private AddGoiVayViewModel viewModel;
        private Plugin.Media.Abstractions.MediaFile imageFile;
        public AddLoanView(BottomModal modal)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddGoiVayViewModel();
            InitSync(modal);
            Init();
            InitAdd();
            loadingPopup.IsVisible = false;
        }

        public AddLoanView(BottomModal modal, Guid Id)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddGoiVayViewModel();
            InitSync(modal);
            Init();
            InitUpdate(Id);
            loadingPopup.IsVisible = false;
        }

        public void InitSync(BottomModal modal)
        {
            this.MaxTimeLookUp.ItemsSource = viewModel.MaxTimeOptions;
            this.MaxTimeUnitLookUp.ItemsSource = viewModel.MaxTimeUnitOptions;
            this.MaxTimeLookUp.BottomModal = this.MaxTimeUnitLookUp.BottomModal = modal;
            this.MaxTimeUnitLookUp.HideClearButton();
        }
        public async void Init()
        {

        }
        private async Task LoadBank()
        {
            var response = await ApiHelper.Get<BankEmployee>(ApiRouter.BANK_EMPLOYEE_DETAIL + UserLogged.Id);
            var employee = response.Content as BankEmployee;
            LblNganHang.Text = employee.Bank.FullName;
            viewModel.BankId = employee.BankId;
        }
        public async void InitAdd()
        {
            //EntryMaxPrice.SetPrice(0);
            //EntryLaiSuat.SetPrice(0);
            viewModel.MaxTimeUnitOption = viewModel.MaxTimeUnitOptions[0];
            await LoadBank();
        }
        public async void InitUpdate(Guid Id)
        {
            this.viewModel.GoiVayModel.Id = Id;

            var response = await ApiHelper.Get<GoiVay>(ApiRouter.BANK_GOIVAY + "/" + Id);
            if (response.IsSuccess)
            {
                var goivay = response.Content as GoiVay;
                viewModel.GoiVayModel.Id = goivay.Id;

                viewModel.BankId = goivay.BankId;
                LblNganHang.Text = goivay.Bank.Name;

                viewModel.GoiVayModel.Name = goivay.Name;

                viewModel.GoiVayModel.LaiSuat = goivay.LaiSuat;
                EntryLaiSuat.SetPrice(goivay.LaiSuat);

                viewModel.GoiVayModel.MaxPrice = goivay.MaxPrice;
                EntryMaxPrice.SetPrice(goivay.MaxPrice);

                viewModel.MaxTimeOption = viewModel.MaxTimeOptions.Where(x => x.Id == goivay.MaxTime).SingleOrDefault();
                viewModel.MaxTimeUnitOption = viewModel.MaxTimeUnitOptions.Where(x => x.Id == goivay.MaxTimeUnit).SingleOrDefault();

                viewModel.GoiVayModel.Condition = goivay.Condition;
                viewModel.GoiVayModel.Description = goivay.Description;

                if (goivay.Image != null)
                {
                    viewModel.GoiVayModel.Image = goivay.Image;
                    ImageGoiVay.Source = goivay.ImageFullUrl;
                    BtnRemoveImage.IsVisible = true;
                    grImageGoiVay.IsVisible = true;
                }
            }
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.GoiVayModel.Name))
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_ten_goi_vay, Language.dong);
                return;
            }

            if (viewModel.MaxTimeOption == null)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_thoi_han_vay_toi_da, Language.dong);
                return;
            }

            if (!EntryLaiSuat.Price.HasValue || EntryLaiSuat.Price.Value == 0)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_lai_suat, Language.dong);
                return;
            }

            if (!EntryMaxPrice.Price.HasValue || EntryMaxPrice.Price.Value == 0)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_thoi_han_vay_toi_da, Language.dong);
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.GoiVayModel.Condition))
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_dieu_kien_vay, Language.dong);
                return;
            }

            // co chon hinh khac.
            if (imageFile != null)
            {
                string fileName = Guid.NewGuid() + ".jpg";
                StreamContent content = new StreamContent(imageFile.GetStream());
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(content);
                var uploadImageResopnse = await UploadImage(form);
                if (uploadImageResopnse.IsSuccess)
                {
                    viewModel.GoiVayModel.Image = fileName;
                }
            }

            GoiVay goivay = new GoiVay();
            goivay.BankId = viewModel.BankId;
            goivay.Name = viewModel.GoiVayModel.Name;
            goivay.MaxTime = viewModel.MaxTimeOption.Id;
            goivay.MaxTimeUnit = viewModel.MaxTimeUnitOption.Id;
            goivay.MaxPrice = EntryMaxPrice.Price.Value;
            goivay.LaiSuat = EntryLaiSuat.Price.Value;
            goivay.Condition = viewModel.GoiVayModel.Condition;
            goivay.Description = viewModel.GoiVayModel.Description;
            goivay.Image = viewModel.GoiVayModel.Image;


            ApiResponse response = null;
            loadingPopup.IsVisible = true;
            if (viewModel.GoiVayModel.Id == Guid.Empty)
            {
                response = await ApiHelper.Post(ApiRouter.BANK_GOIVAY, goivay, true);
            }
            else
            {
                goivay.Id = viewModel.GoiVayModel.Id;
                response = await ApiHelper.Put(ApiRouter.BANK_GOIVAY, goivay, true);
            }

            if (response.IsSuccess)
            {
                this.OnSaved?.Invoke(this, EventArgs.Empty);
                loadingPopup.IsVisible = false;
                MessagingCenter.Send<AddLoanView>(this, "OnSave");
                ToastMessageHelper.ShortMessage(Language.luu_thanh_cong);
            }
            else
            {
                await Shell.Current.DisplayAlert("", response.Message, Language.dong);
                loadingPopup.IsVisible = false;
            }
            loadingPopup.IsVisible = false;
        }

        private async void PickImage_Clicked(object sender, EventArgs e)
        {
            string action = await Shell.Current.DisplayActionSheet(Language.chon_hinh_dai_dien, Language.huy, null, Language.thu_vien_anh, Language.chup_hinh);
            if (action == Language.chup_hinh)
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (cameraStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_may_anh, Language.sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_goi_vay, Language.dong);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        cameraStatus = await PermissionHelper.CheckPermissions(Permission.Camera, Language.quyen_truy_cap_may_anh, Language.sundihome_can_truy_cap_vao_may_anh_de_chup_hinh_goi_vay);
                    }
                    else
                    {
                        var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        if (response.ContainsKey(Permission.Camera))
                            cameraStatus = response[Permission.Camera];
                    }
                }
                if (cameraStatus == PermissionStatus.Granted)
                {
                    await CrossMedia.Current.Initialize();
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await Shell.Current.DisplayAlert("", Language.may_anh_khong_kha_dung, Language.dong);
                        return;
                    }

                    imageFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        SaveToAlbum = false
                    });
                    if (imageFile != null)
                    {
                        ImageGoiVay.Source = imageFile.Path;
                        BtnRemoveImage.IsVisible = true;
                        grImageGoiVay.IsVisible = true;
                    }
                }
            }
            if (action == Language.thu_vien_anh)
            {
                var photoStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                if (photoStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_thu_vien, Language.sundihome_can_truy_cap_vao_thu_vien_de_lay_hinh_anh_goi_vay, Language.dong);
                    }
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        photoStatus = await PermissionHelper.CheckPermissions(Permission.Photos, Language.quyen_truy_cap_thu_vien, Language.sundihome_can_truy_cap_vao_thu_vien_de_lay_hinh_anh_goi_vay);
                    }
                    else
                    {
                        var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                        if (response.ContainsKey(Permission.Photos))
                        {
                            photoStatus = response[Permission.Photos];
                        }
                    }
                }
                if (photoStatus == PermissionStatus.Granted)
                {
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await Shell.Current.DisplayAlert(Language.quyen_truy_cap_bi_tu_choi, Language.khong_the_truy_cap_vao_thu_vien_anh, Language.dong);
                        return;
                    }

                    imageFile = await CrossMedia.Current.PickPhotoAsync();
                    if (imageFile != null)
                    {
                        ImageGoiVay.Source = imageFile.Path;
                        BtnRemoveImage.IsVisible = true;
                        grImageGoiVay.IsVisible = true;
                    }
                }
            }
        }

        private async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=bank/goivay", form);
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
            return uploadResonse;
        }

        private void Remove_Image(object sender, EventArgs e)
        {
            imageFile = null;
            ImageGoiVay.Source = null;
            BtnRemoveImage.IsVisible = false;
            grImageGoiVay.IsVisible = false;
            viewModel.GoiVayModel.Image = null;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            this.OnCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
