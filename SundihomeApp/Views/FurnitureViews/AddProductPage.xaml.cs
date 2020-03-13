using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Models.Furniture;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.Furniture;
using Xamarin.Forms;
using System.Linq;
using Newtonsoft.Json;
using SundihomeApp.Resources;

namespace SundihomeApp.Views.Furniture
{
    public partial class AddProductPage : ContentPage
    {
        private AddProductPageViewModel viewModel;
        private Guid _productId;
        private Guid _companyId;
        private bool _isPromotion;
        //them sp
        public AddProductPage(bool IsPromotion = false)
        {
            Init();
            _isPromotion = IsPromotion;
            InitAdd();
        }
        //them sp cty
        public AddProductPage(Guid companyId)
        {
            Init();
            _companyId = companyId;
            InitAdd();
        }
        //sua sp
        public AddProductPage(Guid productId, bool isUpdateProduct)
        {
            Init();
            _productId = productId;
            InitUpdate();
        }

        private async void Init()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddProductPageViewModel();
            viewModel.AddProductModel = new AddProductModel();
        }

        public async void InitAdd()
        {
            await Task.WhenAll(viewModel.LoadParentCategories(), viewModel.GetProvinceAsync());
            chkUserAddress.CheckedChanged += (sender, e) => OnUserAddressCheckedChanged(sender, e);
            chkCompanyAddress.CheckedChanged += (sender, e) => OnCompanyAddressCheckedChanged(sender, e);
            if (_companyId != Guid.Empty)
            {
                chkCompanyAddress.IsChecked = true;
                await viewModel.GetCompany(_companyId);
            }
            else
            {
                chkUserAddress.IsChecked = true;
                if (!string.IsNullOrEmpty(UserLogged.CompanyId))
                {
                    await viewModel.GetCompany(Guid.Parse(UserLogged.CompanyId));
                }
                else
                {
                    chkCompanyAddress.IsEnabled = false;
                }
            }
            loadingPopup.IsVisible = false;

            viewModel.DateNow = DateTime.Now;
            viewModel.AddProductModel.Status = true;
            chkNew.IsChecked = true;
            viewModel.AddProductModel.IsPromotion = _isPromotion;
            viewModel.AddProductModel.PromotionFromDate = DateTime.Now;
            viewModel.AddProductModel.PromotionToDate = DateTime.Now;


        }
        public async void InitUpdate()
        {
            await viewModel.GetProduct(_productId);
            await Task.WhenAll(viewModel.LoadParentCategories(), viewModel.GetProvinceAsync());
            viewModel.AddProductModel = new AddProductModel()
            {
                Name = viewModel.Product.Name,
                Price = viewModel.Product.Price,
                Status = viewModel.Product.Status,
                Description = viewModel.Product.Description,
                CreatedDate=viewModel.Product.CreatedDate,
                WardId = viewModel.Product.WardId,
                DistrictId = viewModel.Product.DistrictId,
                ProvinceId = viewModel.Product.ProvinceId,
                Street = viewModel.Product.Street,
                Address = viewModel.Product.Address,
                Model = viewModel.Product.Model,
                Origin = viewModel.Product.Origin,
                Guarantee = viewModel.Product.Guarantee,
                ProductStatus = viewModel.Product.ProductStatus,
                IsPromotion = viewModel.Product.IsPromotion,
                PromotionFromDate = viewModel.Product.PromotionFromDate,
                PromotionToDate = viewModel.Product.PromotionToDate,
                PromotionPrice = viewModel.Product.PromotionPrice,
            };

            if (viewModel.Product.CompanyId.HasValue)
            {
                viewModel.Company = viewModel.Product.Company;
                chkCompanyAddress.IsChecked = true;
            }
            else
            {
                chkUserAddress.IsChecked = true;
                if (!string.IsNullOrEmpty(UserLogged.CompanyId))
                {
                    await viewModel.GetCompany(Guid.Parse(UserLogged.CompanyId));
                }
                else
                {
                    CompanyAddress.IsVisible = false;
                }

            }

            await GetAddress();

            //get parent category
            viewModel.ParentCategory = viewModel.ParentCategories.SingleOrDefault(x => x.Id == viewModel.Product.ParentCategoryId);

            //get category
            await viewModel.LoadChildCategories();
            viewModel.ChildCategory = viewModel.ChildCategories.SingleOrDefault(x => x.Id == viewModel.Product.CategoryId);

            //get status
            if (viewModel.Product.Status.HasValue)
            {
                if (viewModel.Product.Status.Value)
                    chkNew.IsChecked = true;
                else
                {
                    chkOld.IsChecked = true;
                }
            }

            //get images
            if (string.IsNullOrEmpty(viewModel.Product.Images) == false)
            {
                string[] imageList = viewModel.Product.Images.Split(',');
                foreach (var image in imageList)
                {
                    viewModel.Media.Add(new MediaFile()
                    {
                        PreviewPath = ImageHelper.GetImageUrl("furniture/product", image)
                    });
                }
            }

            chkUserAddress.CheckedChanged += (sender, e) => OnUserAddressCheckedChanged(sender, e);
            chkCompanyAddress.CheckedChanged += (sender, e) => OnCompanyAddressCheckedChanged(sender, e);

            loadingPopup.IsVisible = false;
        }

        private async void OnParentCategory_Changed(object sender, EventArgs e)
        {
            viewModel.ChildCategory = null;
            viewModel.ChildCategories.Clear();
            if (viewModel.ParentCategory != null)
            {
                await viewModel.LoadChildCategories();
            }
        }

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {

            if (viewModel.AddProductModel.Province != null)
            {
                viewModel.AddProductModel.ProvinceId = viewModel.AddProductModel.Province.Id;
            }
            else
            {
                viewModel.AddProductModel.ProvinceId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.AddProductModel.District = null;
            viewModel.AddProductModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.AddProductModel.District != null)
            {
                viewModel.AddProductModel.DistrictId = viewModel.AddProductModel.District.Id;
            }
            else
            {
                viewModel.AddProductModel.DistrictId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.AddProductModel.Ward = null;

            loadingPopup.IsVisible = false;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {

            if (viewModel.ParentCategory == null)
            {
                await DisplayAlert("", Language.chon_nhom_danh_muc, Language.dong);
                return;
            }

            if (viewModel.ChildCategory == null)
            {
                await DisplayAlert("", Language.chon_danh_muc, Language.dong);
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.AddProductModel.Name))
            {
                await DisplayAlert("", Language.nhap_ten_noi_that, Language.dong);
                return;
            }

            if (!viewModel.AddProductModel.Price.HasValue)
            {
                await DisplayAlert("", Language.nhap_gia_san_pham, Language.dong);
                return;
            }

            if (viewModel.AddProductModel.IsPromotion == true)
            {
                if (viewModel.AddProductModel.PromotionFromDate == null)
                {
                    await DisplayAlert("", Language.vui_long_chon_thoi_gian, Language.dong);
                    return;
                }
                if (viewModel.AddProductModel.PromotionToDate == null)
                {
                    await DisplayAlert("", Language.vui_long_chon_thoi_gian, Language.dong);
                    return;
                }
                if (viewModel.AddProductModel.PromotionPrice == null)
                {
                    await DisplayAlert("", Language.vui_long_nhap_gia_giam, Language.dong);
                    return;
                }

            }

            if (viewModel.AddProductModel.Province == null)
            {
                await DisplayAlert("",Language.vui_long_chon_tinh_thanh,Language.dong);
                return;
            }
            if (viewModel.AddProductModel.District == null)
            {
                await DisplayAlert("",Language.vui_long_chon_quan_huyen,Language.dong);
                return;
            }
            if (viewModel.AddProductModel.Ward == null)
            {
                await DisplayAlert("",Language.vui_long_chon_phuong_xa,Language.dong);
                return;
            }

            

            if (viewModel.Media.Count < 1)
            {
                await DisplayAlert("", Language.chon_hinh_anh_cho_san_pham, Language.dong);
                return;
            }


            loadingPopup.IsVisible = true;
             
            FurnitureProduct product = new FurnitureProduct();
            product.Name = viewModel.AddProductModel.Name;
            product.ParentCategoryId = viewModel.ParentCategory.Id;
            product.CategoryId = viewModel.ChildCategory.Id;
            product.CreatedById = Guid.Parse(UserLogged.Id);
            product.Price = viewModel.AddProductModel.Price;
            product.Status = viewModel.AddProductModel.Status;
            product.Model = viewModel.AddProductModel.Model;
            product.Origin = viewModel.AddProductModel.Origin;
            product.Guarantee = viewModel.AddProductModel.Guarantee;
            product.Description = viewModel.AddProductModel.Description;
            product.CreatedDate = viewModel.AddProductModel.CreatedDate;

            product.ProductStatus = 0;
            product.IsPromotion = viewModel.AddProductModel.IsPromotion;
            if (viewModel.AddProductModel.IsPromotion == true)
            {
                product.PromotionFromDate = viewModel.AddProductModel.PromotionFromDate;
                product.PromotionToDate = viewModel.AddProductModel.PromotionToDate;
                product.PromotionPrice = viewModel.AddProductModel.PromotionPrice;
            }


            if (viewModel.AddProductModel.Province != null)
            {
                product.ProvinceId = viewModel.AddProductModel.Province.Id;
            }
            if (viewModel.AddProductModel.District != null)
            {
                product.DistrictId = viewModel.AddProductModel.District.Id;
            }
            if (viewModel.AddProductModel.Ward != null)
            {
                product.WardId = viewModel.AddProductModel.Ward.Id;
            }

            product.Street = viewModel.AddProductModel.Street;
            product.Address = viewModel.AddProductModel.Address;

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
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(content);
                    var response = await BsdHttpClient.Instance().PostAsync(ApiRouter.FURNITUREPRODUCT_IMAGE_UPLOAD, form);
                    if (!response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("", Language.khong_the_upload_hinh_anh, Language.dong);
                        loadingPopup.IsVisible = false;
                        return;
                    }
                }
                else
                {
                    imageList[i] = item.PreviewPath.Replace(Configuration.ApiConfig.CloudStorageApiCDN + "/furniture/product/", "");
                }
            }

            product.AvatarUrl = imageList[0];
            product.Images = string.Join(",", imageList);

            if (chkCompanyAddress.IsChecked && viewModel.Company != null) //sp cty
            {
                product.CompanyId = viewModel.Company.Id;
            }

            //save
            ApiResponse apiResponse = null;

            if (_productId == Guid.Empty)//add
            {
                apiResponse = await ApiHelper.Post(ApiRouter.FURNITUREPRODUCT_ADD_UPDATE, product, true);
            }
            else//upd
            {
                product.Id = _productId;
                apiResponse = await ApiHelper.Put($"{ApiRouter.FURNITUREPRODUCT_ADD_UPDATE}/update", product, true);
            }

            if (apiResponse.IsSuccess)
            {
                var responseProduct = JsonConvert.DeserializeObject<FurnitureProduct>(apiResponse.Content.ToString());
                product.Id = responseProduct.Id;
                await Navigation.PopAsync();

                if (_productId == Guid.Empty)
                {
                    MessagingCenter.Send<AddProductPage, bool>(this, "AddProduct", (bool)viewModel.AddProductModel.IsPromotion);
                    ToastMessageHelper.ShortMessage(Language.dang_thanh_cong);
                }
                else
                {
                    MessagingCenter.Send<AddProductPage, FurnitureProduct>(this, "UpdateProduct", product);
                    ToastMessageHelper.ShortMessage(Language.luu_san_pham_thanh_cong);
                }
                
            }
            else
            {
                ToastMessageHelper.ShortMessage($"{Language.loi}, {apiResponse.Message}");
            }
            loadingPopup.IsVisible = false;
        }

        public void OnStatusNewCheckedTapped(object sender, EventArgs e)
        {
            viewModel.AddProductModel.Status = true;
            chkOld.IsChecked = false;
            chkNew.IsChecked = true;
        }

        public void OnStatusOldCheckedTapped(object sender, EventArgs e)
        {
            viewModel.AddProductModel.Status = false;
            chkNew.IsChecked = false;
            chkOld.IsChecked = true;
        }

        public void OnStatusNewCheckedChanged(object sender, EventArgs e)
        {

        }

        public void OnStatusOldCheckedChanged(object sender, EventArgs e)
        {

        }

        public void OnUserAddressCheckedTapped(object sender, EventArgs e)
        {
            chkUserAddress.IsChecked = !chkUserAddress.IsChecked;
        }

        public void OnCompanyAddressCheckedTapped(object sender, EventArgs e)
        {
            chkCompanyAddress.IsChecked = !chkCompanyAddress.IsChecked;
        }

        public async void OnUserAddressCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                chkCompanyAddress.IsChecked = false;
                await GetUserAddress();
            }
            else if (!chkCompanyAddress.IsChecked)
            {
                chkUserAddress.IsChecked = true;
            }
        }

        public async void OnCompanyAddressCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                chkUserAddress.IsChecked = false;
                await GetCompanyAddress();
            }
            else if (!chkUserAddress.IsChecked)
            {
                chkCompanyAddress.IsChecked = true;
            }
        }

        //
        public async Task GetAddress()
        {
            viewModel.AddProductModel.ProvinceId = viewModel.Product.ProvinceId;
            viewModel.AddProductModel.DistrictId = viewModel.Product.DistrictId;
            viewModel.AddProductModel.WardId = viewModel.Product.WardId;
            await Task.WhenAll(viewModel.GetDistrictAsync(),
                viewModel.GetWardAsync());
            viewModel.AddProductModel.Province = viewModel.ProvinceList.SingleOrDefault(x => x.Id == viewModel.Product.ProvinceId);
            viewModel.AddProductModel.District = viewModel.DistrictList.SingleOrDefault(x => x.Id == viewModel.Product.DistrictId);
            viewModel.AddProductModel.Ward = viewModel.WardList.SingleOrDefault(x => x.Id == viewModel.Product.WardId);
            viewModel.AddProductModel.Street = viewModel.Product.Street;
        }

        public async Task GetCompanyAddress()
        {
            viewModel.AddProductModel.ProvinceId = viewModel.Company.ProvinceId;
            viewModel.AddProductModel.DistrictId = viewModel.Company.DistrictId;
            viewModel.AddProductModel.WardId = viewModel.Company.WardId;
            await Task.WhenAll(viewModel.GetDistrictAsync(),
                viewModel.GetWardAsync());
            viewModel.AddProductModel.Province = viewModel.ProvinceList.SingleOrDefault(x => x.Id == viewModel.Company.ProvinceId);
            viewModel.AddProductModel.District = viewModel.DistrictList.SingleOrDefault(x => x.Id == viewModel.Company.DistrictId);
            viewModel.AddProductModel.Ward = viewModel.WardList.SingleOrDefault(x => x.Id == viewModel.Company.WardId);
            viewModel.AddProductModel.Street = viewModel.Company.Street;
        }

        public async Task GetUserAddress()
        {
            if (UserLogged.ProvinceId != -1 || UserLogged.DistrictId != -1 ||
                UserLogged.WardId != -1 || !string.IsNullOrEmpty(UserLogged.Street))
            {
                viewModel.AddProductModel.ProvinceId = UserLogged.ProvinceId;
                viewModel.AddProductModel.DistrictId = UserLogged.DistrictId;
                viewModel.AddProductModel.WardId = UserLogged.WardId;
                await Task.WhenAll(viewModel.GetDistrictAsync(),
                    viewModel.GetWardAsync());
                viewModel.AddProductModel.Province = viewModel.ProvinceList.SingleOrDefault(x => x.Id == UserLogged.ProvinceId);
                viewModel.AddProductModel.District = viewModel.DistrictList.SingleOrDefault(x => x.Id == UserLogged.DistrictId);
                viewModel.AddProductModel.Ward = viewModel.WardList.SingleOrDefault(x => x.Id == UserLogged.WardId);
                viewModel.AddProductModel.Street = UserLogged.Street;
            }
            else
            {
                if (UserLogged.ProvinceId == -1)
                {
                    viewModel.AddProductModel.Province = null;
                }
                if (UserLogged.DistrictId == -1)
                {
                    viewModel.AddProductModel.District = null;
                    viewModel.DistrictList.Clear();
                }
                if (UserLogged.WardId == -1)
                {
                    viewModel.AddProductModel.Ward = null;
                    viewModel.WardList.Clear();
                }
                if (string.IsNullOrEmpty(UserLogged.Street))
                {
                    viewModel.AddProductModel.Street = null;
                }
                if (!string.IsNullOrEmpty(UserLogged.Address))
                {
                    viewModel.AddProductModel.Address = UserLogged.Address;
                }
            }
        }

        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }
    }
}
