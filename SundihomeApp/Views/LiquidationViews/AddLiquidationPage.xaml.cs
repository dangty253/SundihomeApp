using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Models.LiquidationModel;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class AddLiquidationPage : ContentPage
    {
        private int type = 0;
        public AddLiquidationPageViewModel viewModel;
        private Guid liquidationId;
        public AddLiquidationPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddLiquidationPageViewModel();
            Init();
            InitAdd();
        }

        public async void InitAdd()
        {
            Title = Language.dang_thanh_ly;
            viewModel.LiquidationModel = new AddLiquidationModel();
            await viewModel.GetProvinceAsync();

            if (UserLogged.ProvinceId != -1)
            {
                var selectedProvince = viewModel.ProvinceList.SingleOrDefault(x => x.Id == UserLogged.ProvinceId);
                if (selectedProvince != null)
                {
                    this.viewModel.LiquidationModel.ProvinceId = selectedProvince.Id;
                    this.viewModel.LiquidationModel.Province = selectedProvince;


                    await viewModel.GetDistrictAsync();
                }
            }

            if (UserLogged.DistrictId != -1)
            {
                var selectedDistrict = viewModel.DistrictList.SingleOrDefault(x => x.Id == UserLogged.DistrictId);
                if (selectedDistrict != null)
                {
                    this.viewModel.LiquidationModel.DistrictId = selectedDistrict.Id;
                    this.viewModel.LiquidationModel.District = selectedDistrict;

                    await viewModel.GetWardAsync();
                }
            }

            if (UserLogged.WardId != -1)
            {
                var selectedWard = viewModel.WardList.SingleOrDefault(x => x.Id == UserLogged.WardId);
                if (selectedWard != null)
                {
                    this.viewModel.LiquidationModel.WardId = selectedWard.Id;
                    this.viewModel.LiquidationModel.Ward = selectedWard;
                }
            }
            if (string.IsNullOrEmpty(UserLogged.Street) == false)
            {
                this.viewModel.LiquidationModel.Street = UserLogged.Street;
            }
            if (string.IsNullOrEmpty(UserLogged.Address) == false)
            {
                this.viewModel.LiquidationModel.Address = UserLogged.Address;
            }
            loadingPopup.IsVisible = false;
        }

        public AddLiquidationPage(Guid LiquidationId)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddLiquidationPageViewModel();
            liquidationId = LiquidationId;
            Init();
            InitUpdate();
        }

        public async void Init()
        {
            await CrossMedia.Current.Initialize();
        }
        public async void InitUpdate()
        {
            StackLayoutStatus.IsVisible = true;

            ApiResponse response = await ApiHelper.Get<Liquidation>(ApiRouter.LIQUIDATION_GETBYID + "/" + this.liquidationId + "?IncludeProvince=true");
            if (response.IsSuccess)
            {
                Liquidation liquidation = response.Content as Liquidation;
                Title = liquidation.Name;
                viewModel.LiquidationModel = new AddLiquidationModel()
                {
                    Id = liquidation.Id,
                    Code = liquidation.Code,
                    Name = liquidation.Name,
                    Description = liquidation.Description,
                    Price = liquidation.Price,
                    LiquidationCategory = viewModel.LiquidationCategories.SingleOrDefault(x => x.Id == liquidation.CategoryId),
                    Province = liquidation.Province,
                    ProvinceId = liquidation.ProvinceId,
                    District = liquidation.District,
                    DistrictId = liquidation.DistrictId,
                    Ward = liquidation.Ward,
                    WardId = liquidation.WardId,
                    Street = liquidation.Street,
                    Address = liquidation.Address
                };
                await Task.WhenAll(
                    viewModel.GetProvinceAsync(),
                    viewModel.GetDistrictAsync(),
                    viewModel.GetWardAsync()
                );

                string[] imageList = liquidation.Images.Split(',');
                foreach (var image in imageList)
                {
                    viewModel.Media.Add(new MediaFile()
                    {
                        PreviewPath = ImageHelper.GetImageUrl("liquidation", image),
                    });
                }

                if (liquidation.Type == 1)
                {
                    TypeChoTang_Checked(null, EventArgs.Empty); //set type =1 
                }
                LblStatus.Text = liquidation.StatusFormat;



                loadingPopup.IsVisible = false;
            }
        }

        

        private void TypeThanhLy_Checked(object sender, EventArgs e)
        {
            type = 0;
            CheckBoxThanhLy.IsChecked = true;
            CheckBoxChoTang.IsChecked = false;
            StackLayoutPrice.IsVisible = true;
        }
        private void TypeChoTang_Checked(object sender, EventArgs e)
        {
            type = 1;
            StackLayoutPrice.IsVisible = false;
            CheckBoxChoTang.IsChecked = true;
            CheckBoxThanhLy.IsChecked = false;
            viewModel.LiquidationModel.Price = 0;
        }

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {

            if (viewModel.LiquidationModel.Province != null)
            {
                viewModel.LiquidationModel.ProvinceId = viewModel.LiquidationModel.Province.Id;
            }
            else
            {
                viewModel.LiquidationModel.ProvinceId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.LiquidationModel.District = null;
            viewModel.LiquidationModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.LiquidationModel.District != null)
            {
                viewModel.LiquidationModel.DistrictId = viewModel.LiquidationModel.District.Id;
            }
            else
            {
                viewModel.LiquidationModel.DistrictId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.LiquidationModel.Ward = null;

            loadingPopup.IsVisible = false;
        }

        public void ClearDescription_Clicked(object sender, EventArgs e)
        {
            viewModel.LiquidationModel.Description = null;
        }
        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }

        public async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=liquidation", form);

            string body = await uploadResponse.Content.ReadAsStringAsync();
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(body);
            return uploadResonse;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (viewModel.LiquidationModel.LiquidationCategory == null)
            {
                await DisplayAlert("", Language.vui_long_chon_danh_muc_san_pham, Language.dong);
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.LiquidationModel.Name))
            {
                await DisplayAlert("", Language.vui_long_nhap_ten_san_pham, Language.dong);
                return;
            }
            if (type == 0 && viewModel.LiquidationModel.Price == 0)
            {
                await DisplayAlert("",Language.vui_long_nhap_gia_thanh_ly, Language.dong);
                return;
            }
            if (viewModel.LiquidationModel.Province == null || viewModel.LiquidationModel.District == null || viewModel.LiquidationModel.Ward == null)
            {
                await DisplayAlert("", Language.vui_long_cung_cap_thong_tin_dia_chi, Language.dong);
                return;
            }

            if (viewModel.Media.Any() == false)
            {
                await DisplayAlert("", Language.vui_long_chon_hinh_anh_san_pham, Language.dong);
                return;
            }

            loadingPopup.IsVisible = true;

            bool ImageUploaded = true;
            MultipartFormDataContent form = new MultipartFormDataContent();
            string[] imageList = new string[viewModel.Media.Count];
            for (int i = 0; i < viewModel.Media.Count; i++)
            {
                var item = viewModel.Media[i];
                // chua upload. upload roi link = null 
                if (string.IsNullOrEmpty(item.Path) == false) // co link la co chon tu dien thoai.
                {
                    imageList[i] = $"{Guid.NewGuid().ToString()}.jpg";
                    var stream = new MemoryStream(File.ReadAllBytes(item.Path));
                    var content = new StreamContent(stream);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "files" + i,
                        FileName = imageList[i]
                    };
                    form.Add(content);
                }
                else
                {
                    imageList[i] = item.PreviewPath.Replace(ApiConfig.CloudStorageApiCDN + "/liquidation/", "");
                }
            }

            // bat dau up hinh.
            if (viewModel.Media.Any(x => x.Path != null))
            {
                ApiResponse uploadImageResponse = await UploadImage(form);
                if (!uploadImageResponse.IsSuccess)
                {
                    await DisplayAlert("", Language.hinh_anh_vuot_qua_dung_luong_vui_long_thu_lai, Language.dong);
                    ImageUploaded = false;
                    loadingPopup.IsVisible = false;
                }
            }

            if (ImageUploaded)
            {
                Liquidation liquidation = new Liquidation();
                liquidation.Id = viewModel.LiquidationModel.Id;
                liquidation.Name = viewModel.LiquidationModel.Name;
                liquidation.Code = viewModel.LiquidationModel.Code;
                liquidation.CategoryId = viewModel.LiquidationModel.LiquidationCategory.Id;
                liquidation.Type = type;
                liquidation.Price = type == 0 ? viewModel.LiquidationModel.Price : 0;
                liquidation.Status = 0;
                liquidation.Description = viewModel.LiquidationModel.Description;
                liquidation.ProvinceId = viewModel.LiquidationModel.Province.Id;
                liquidation.DistrictId = viewModel.LiquidationModel.District.Id;
                liquidation.WardId = viewModel.LiquidationModel.Ward.Id;
                liquidation.Street = viewModel.LiquidationModel.Street;
                liquidation.Address = viewModel.LiquidationModel.Address;
                liquidation.Avatar = imageList[0];
                liquidation.Images = string.Join(",", imageList);

                ApiResponse apiResponse = null;
                if (liquidationId == Guid.Empty)
                {
                    liquidationId = liquidation.Id;
                    apiResponse = await ApiHelper.Post(ApiRouter.LIQUIDATION_SAVE, liquidation, true);
                }
                else
                {
                    apiResponse = await ApiHelper.Put(ApiRouter.LIQUIDATION_SAVE, liquidation, true);
                }

                if (apiResponse.IsSuccess)
                {
                    try
                    {
                        await Navigation.PopAsync(false);
                        MessagingCenter.Send<AddLiquidationPage>(this, "OnSaveItem");
                        MessagingCenter.Send<AddLiquidationPage, Guid>(this, "OnSaveItem", liquidation.Id);
                        ToastMessageHelper.ShortMessage(Language.luu_san_pham_thanh_cong);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    loadingPopup.IsVisible = false;
                    await DisplayAlert("", Language.khong_the_luu_san_pham_vui_long_thu_lai, Language.dong);
                }
            }
            loadingPopup.IsVisible = false;
        }
    }
}
