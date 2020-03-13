using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class AddContactNeedContent : ContentView
    {
        public AddContactNeedContentViewModel viewModel; 
        public static readonly BindableProperty IdProperty =
            BindableProperty.Create(nameof(Id), typeof(Guid), typeof(AddContactNeedContent));
        public Guid Id
        {
            get => (Guid)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public AddContactNeedContent(Contact contact)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddContactNeedContentViewModel();
            ModalAddContactNeed.Body.BindingContext = viewModel;
            viewModel._contact = contact;
            Init();
            loadingPopup.IsVisible = false;
        }
        public async void Init()
        {
            //InitUpdate();
            Decimal_nganSach.SetPrice(0);
            InitLoaiBatDongSanList();
            await viewModel.GetProviceAsync();
            await viewModel.LoadData();
            LvData.ItemTapped += LvData_ItemTapped;
            ModalAddContactNeed.CustomCloseButton(OnBtnCancelAddContactNeed_Click);

        }
        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var contactNeed = e.Item as ContactNeed;
            ModalAddContactNeed.Title = Language.sua_thong_tin_nhu_cau;
            viewModel.ContactNeedModel.Province = viewModel.ProvinceList.Single(x => x.Id == contactNeed.ProvinceId);
            viewModel.ContactNeedModel.ProvinceId = viewModel.ContactNeedModel.Province.Id;
            await viewModel.GetDistrictAsync();
            viewModel.ContactNeedModel.District = viewModel.DistrictList.Single(x => x.Id == contactNeed.DistrictId);
            viewModel.ContactNeedModel.DistrictId = viewModel.ContactNeedModel.District.Id;
            viewModel.ContactNeedModel.Project = contactNeed.Project;
            viewModel.LoaiBatDongSanSelected = Models.LoaiBatDongSanModel.GetList(null).SingleOrDefault(x => x.Id == contactNeed.Type);
            viewModel.ContactNeedModel.Id = contactNeed.Id;
            viewModel.ContactNeedModel.Description = contactNeed.Description;
            viewModel.ContactNeedModel.Piority = contactNeed.Piority;
            if (contactNeed.Piority.HasValue) DoUuTien.Value = (int)contactNeed.Piority;
            viewModel.ContactNeedModel.Rate = contactNeed.Rate;
            if(contactNeed.Rate.HasValue) DanhGiaKhaNang.Value = (int)contactNeed.Rate;
            viewModel.ContactNeedModel.Budget = contactNeed.Budget;
            Decimal_nganSach.SetPrice(contactNeed.Budget);
            await ModalAddContactNeed.Show();
        }
        public async void InitUpdate()
        {

            var apiResponse = await ApiHelper.Get<ContactNeedModel>($"api/contactneed/{viewModel._contact.Id}", true);
            if (apiResponse.IsSuccess == false) return;

            var model = apiResponse.Content as ContactNeedModel;
            viewModel.ContactNeedModel = model;


            await Task.WhenAll(
                viewModel.GetProviceAsync(),
                viewModel.GetDistrictAsync()
                );
            //set lai dia chi
            if (model.ProvinceId != null)
            {
                viewModel.ContactNeedModel.Province = viewModel.ProvinceList.Single(x => x.Id == model.ProvinceId);
            }
            if (model.DistrictId != null)
                viewModel.ContactNeedModel.District = viewModel.DistrictList.Single(x => x.Id == model.DistrictId);
        }
        public void InitLoaiBatDongSanList()
        {
            viewModel.LoaiBatDongSanList = LoaiBatDongSanModel.GetList(null);

        }
        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            //chon tinh/thanh
            if (viewModel.ContactNeedModel.Province != null)
            {
                viewModel.ContactNeedModel.ProvinceId = viewModel.ContactNeedModel.Province.Id;
            }
            else
            {
                viewModel.ContactNeedModel.Province = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.ContactNeedModel.District = null;
            loadingPopup.IsVisible = false;
        }
        public void District_Change(object sender, LookUpChangeEvent e)
        {
            //chon quan/huyen
            if (viewModel.ContactNeedModel.District != null)
            {
                viewModel.ContactNeedModel.DistrictId = viewModel.ContactNeedModel.District.Id;
            }
            else
            {
                viewModel.ContactNeedModel.District = null;
            }
        }

        public async void AddContactList_Clicked(Object Sender, EventArgs e)
        {
            ModalAddContactNeed.Title = Language.them_nhu_cau_khach_hang;
            Decimal_nganSach.SetPrice(0);
            DanhGiaKhaNang.Value = 0;
            DoUuTien.Value = 0;
            await ModalAddContactNeed.Show();
        }
        public async void CloseModalAddContactNeed_Clicked(Object Sender, EventArgs e)
        {
            await ModalAddContactNeed.Hide();
        } 

        public async void OnBtnSaveAddContactNeed_Click(object sender, EventArgs e)
        {
            if (viewModel.LoaiBatDongSanSelected == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_chon_loai_hinh_bat_dong_san, Language.dong);
            else if (viewModel.ContactNeedModel.Province == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_chon_tinh_thanh, Language.dong);
            else if (viewModel.ContactNeedModel.District == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_chon_quan_huyen, Language.dong);
            else if (viewModel.ContactNeedModel.Piority == 0) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_chon_do_uu_tien, Language.dong);
            else if (viewModel.ContactNeedModel.Id == Guid.Empty)
            {
                ContactNeed contactNeed = new ContactNeed();
                contactNeed.Project = viewModel.ContactNeedModel.Project;
                contactNeed.ProvinceId = viewModel.ContactNeedModel.ProvinceId;
                contactNeed.DistrictId = viewModel.ContactNeedModel.DistrictId;
                contactNeed.ContactId = viewModel._contact.Id;
                contactNeed.Description = viewModel.ContactNeedModel.Description;
                contactNeed.Type = viewModel.LoaiBatDongSanSelected.Id;
                contactNeed.Budget= Decimal_nganSach.Price.Value;
                contactNeed.Piority = viewModel.ContactNeedModel.Piority;
                contactNeed.Rate = viewModel.ContactNeedModel.Rate;
                ApiResponse apiResponse = await ApiHelper.Post(ApiRouter.CONTACT_ADD_NEED, contactNeed);
                if (apiResponse.IsSuccess)
                {
                    await ModalAddContactNeed.Hide();
                    viewModel.CancelPopUpAddContactNeed();
                    await viewModel.LoadOnRefreshCommandAsync();
                    MessagingCenter.Send<AddContactNeedContent>(this, "AddContactNeed");
                    ToastMessageHelper.ShortMessage(Language.them_nhu_cau_thanh_cong);
                }
                else
                {
                    ToastMessageHelper.ShortMessage(apiResponse.Message);
                }
            }
            else
            {
                ContactNeed contactNeed = new ContactNeed();
                contactNeed.Id = viewModel.ContactNeedModel.Id;
                contactNeed.Project = viewModel.ContactNeedModel.Project;
                contactNeed.ProvinceId = viewModel.ContactNeedModel.ProvinceId;
                contactNeed.DistrictId = viewModel.ContactNeedModel.DistrictId;
                contactNeed.ContactId = viewModel._contact.Id;
                contactNeed.Type = viewModel.LoaiBatDongSanSelected.Id;
                contactNeed.Description = viewModel.ContactNeedModel.Description;
                if (Decimal_nganSach.Price.HasValue) contactNeed.Budget = Decimal_nganSach.Price.Value;
                else contactNeed.Budget = null;
                contactNeed.Piority = viewModel.ContactNeedModel.Piority;
                contactNeed.Rate = viewModel.ContactNeedModel.Rate;

                ApiResponse apiResponse = await ApiHelper.Put(ApiRouter.CONTACT_PUT_NEED, contactNeed);
                if (apiResponse.IsSuccess)
                {
                    await ModalAddContactNeed.Hide();
                    viewModel.CancelPopUpAddContactNeed();
                    await viewModel.LoadOnRefreshCommandAsync();
                    MessagingCenter.Send<AddContactNeedContent, Guid>(this, "UpdateContactNeed", contactNeed.Id);
                    ToastMessageHelper.ShortMessage(Language.sua_thanh_cong);
                }
                else
                {
                    ToastMessageHelper.ShortMessage(apiResponse.Message);
                }
            }
        }
        public async void OnBtnCancelAddContactNeed_Click(object sender, EventArgs e)
        {
            await ModalAddContactNeed.Hide();
            viewModel.CancelPopUpAddContactNeed();
        }
        public async void DeleteContactNeedItemClick(object sender, EventArgs e)
        {
            var answer = await Shell.Current.DisplayAlert(Language.xac_nhan_xoa, Language.luu_y_ban_co_chac_chan_muon_xoa_nhu_cau_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            ContactNeed item = ((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as ContactNeed;
            await viewModel.DeleteContactNeedAsync(item);
        }
        public  void Clear_Clicked(object sender, EventArgs e)
        {
            viewModel.ContactNeedModel.Description = null;
        }
        public  void DoUuTien_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            viewModel.ContactNeedModel.Piority = (int)e.NewValue;
        }
        public  void DanhGiaKhaNang_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            viewModel.ContactNeedModel.Rate = (int)e.NewValue;
        }



    }
}
