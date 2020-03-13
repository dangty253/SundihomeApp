using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class PickerLiquidationPage : ContentPage
    {
        public LiquidationFilterViewModel viewModel;
        private Guid SelectedId;
        private bool _fromProfilePage = false;
        public PickerLiquidationPage(bool FromProfilePage)
        {
            InitializeComponent();
            _fromProfilePage = FromProfilePage;
            this.BindingContext = viewModel = new LiquidationFilterViewModel();
            viewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
            viewModel.FilterModel.Type = 0; // chi thanh ly moi chon
            //viewModel.FilterModel.Status = 0; // trang thai dang ban
            Init();

        }
        public async void Init()
        {
            ModalPrice.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            ListViewThanhLy.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as Liquidation;
                this.SelectedId = item.Id;

                ApiResponse CheckTimeResponse = await ApiHelper.Post(ApiRouter.LIQUIDATIONTODAY_CHECKTIME + "/" + SelectedId, null, true);
                if (CheckTimeResponse.IsSuccess == false)
                {
                    loadingPopup.IsVisible = false;
                    await DisplayAlert("", CheckTimeResponse.Message, Language.dong);
                    return;
                }

                ModalPrice.IsVisible = true;
                EntryPrice.FocusEntry();
            };
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            CloseModal();
        }
        private async void CloseModal()
        {
            ModalPrice.IsVisible = false;
            EntryPrice.Text = null;
        }
        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (EntryPrice.Text.HasValue == false || EntryPrice.Text.Value == 0)
            {
                await DisplayAlert("", Language.vui_long_nhap_gia_thanh_ly_trong_ngay, Language.dong);
                return;
            }
            loadingPopup.IsVisible = true;
            ApiResponse response = await ApiHelper.Get<Liquidation>(ApiRouter.LIQUIDATION_GETBYID + "/" + this.SelectedId);
            Liquidation liquidation = response.Content as Liquidation;

            ApiResponse copyResponse = await this.CopyImage(liquidation.Images);
            if (copyResponse == null)
            {
                await DisplayAlert("", Language.loi_hinh_anh_vui_long_thu_lai_sau, Language.dong);
            }
            else if (copyResponse.IsSuccess == false)
            {
                await DisplayAlert("", Language.loi_hinh_anh_vui_long_thu_lai_sau, Language.dong);
            }
            else
            {
                LiquidationToDay liquidationToDay = new LiquidationToDay()
                {
                    LiquidationId = liquidation.Id, // xac dinh la tu liquation thanh ly qua.
                    Code = liquidation.Code,
                    Name = liquidation.Name,
                    CategoryId = liquidation.CategoryId,
                    Price = liquidation.Price,
                    PriceToDay = EntryPrice.Text.Value,
                    Status = 0,
                    Description = liquidation.Description,
                    ProvinceId = liquidation.ProvinceId,
                    DistrictId = liquidation.DistrictId,
                    WardId = liquidation.WardId,
                    Street = liquidation.Street,
                    Address = liquidation.Address,
                    Avatar = liquidation.Avatar,
                    Images = liquidation.Images
                };

                var addResponse = await ApiHelper.Post(ApiRouter.LIQUIDATIONTODAY_SAVE, liquidationToDay, true);
                if (addResponse.IsSuccess)
                {
                    if (_fromProfilePage)
                    {
                        await Navigation.PopAsync();
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await Navigation.PopToRootAsync(); // tu danh sach qua thi back  ve list..
                    }
                    MessagingCenter.Send<PickerLiquidationPage>(this, "OnSaveItem");
                    ToastMessageHelper.ShortMessage(Language.dang_tin_thanh_ly_thanh_cong);
                }
                else
                {
                    CloseModal();
                    await DisplayAlert("", Language.loi_dang_tin_thanh_ly_vui_long_thu_lai, Language.dong);
                }
            }
            loadingPopup.IsVisible = false;
        }


        private async Task<ApiResponse> CopyImage(string Images)
        {
            var client = BsdHttpClient.Instance();
            HttpResponseMessage uploadResponse = await client.GetAsync(ApiConfig.CloudStorageApi + "/api/files/copy?fromfolder=liquidation&tofolder=liquidation_today&filenames=" + Images);
            if (uploadResponse.IsSuccessStatusCode)
            {
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
                return apiResponse;
            }
            return null;
        }
    }
}
