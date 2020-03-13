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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class ContactDetailContent : ContentView
    {
        public ContactDetailContentViewModel viewModel;
        private Guid _contactId;
        public ContactDetailContent(Guid contactId)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ContactDetailContentViewModel();
            this._contactId = contactId;
            ModalEditContact.Body.BindingContext = viewModel;
            Init();
        }

        public async void Init()
        {
            InitGroupList();
            await InitContact();
            await viewModel.GetProviceAsync();
            SetFloatingButtonGroup();
            loadingPopup.IsVisible = false;

        }

        public async Task InitContact()
        {
            var apiResponse = await ApiHelper.Get<ContactModel>($"api/contact/{this._contactId}", true);
            if (apiResponse.IsSuccess == false) return;

            var model = apiResponse.Content as ContactModel;
            model.SelectGroup = viewModel.GroupList.Single(x => x.Id == model.GroupId);

            viewModel.Contact = model;
        }
        public async Task InitUpdate()
        {
            await viewModel.GetProviceAsync();

            //set lai dia chi
            if (viewModel.Contact.ProvinceId.HasValue)
            {
                await viewModel.GetDistrictAsync(viewModel.Contact.ProvinceId);
            }
            if (viewModel.Contact.DistrictId.HasValue)
            {
                await viewModel.GetWardAsync(viewModel.Contact.DistrictId);
            }
            loadingPopup.IsVisible = false;
        }
        public void InitGroupList()
        {
            var item = new Option();
            item.Id = 0;
            item.Name = Language.moi;
            viewModel.GroupList.Add(item);
            item = new Option();
            item.Id = 1;
            item.Name = Language.dau_tu;
            viewModel.GroupList.Add(item);
            item = new Option();
            item.Id = 2;
            item.Name = Language.da_mua;
            viewModel.GroupList.Add(item);

        }
        public void InitLoaiBatDongSanList()
        {
            viewModel.LoaiBatDongSanList = LoaiBatDongSanModel.GetList(null);

        }
        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            //chon tinh/thanh
            if (viewModel.ContactModel.Province != null)
            {
                viewModel.ContactModel.ProvinceId = viewModel.ContactModel.Province.Id;
                await viewModel.GetDistrictAsync(viewModel.ContactModel.ProvinceId);
            }
            else
            {
                viewModel.ContactModel.ProvinceId = null;
                viewModel.DistrictList.Clear();
            }

            loadingPopup.IsVisible = true;
            
            viewModel.WardList.Clear();
            
            viewModel.ContactModel.District = null;
            viewModel.ContactModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            //chon quan/huyen
            if (viewModel.ContactModel.District != null)
            {
                viewModel.ContactModel.DistrictId = viewModel.ContactModel.District.Id;
                await viewModel.GetWardAsync(viewModel.ContactModel.DistrictId);
            }
            else
            {
                viewModel.ContactModel.DistrictId = null;
                viewModel.WardList.Clear();
            }
            loadingPopup.IsVisible = true;
            
            viewModel.ContactModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void UpdateContact_Clicked(Object Sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (viewModel.ProvinceList.Count != 0)
            {
                await InitUpdate();
            }

            viewModel.ContactModel = new ContactModel(viewModel.Contact);
            loadingPopup.IsVisible = false;
            await ModalEditContact.Show();
        }

        public async void OnBtnSaveContactDetail_Click(object sender, EventArgs e)
        {

            if (viewModel.ContactModel.FullName == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_nhap_ten_khach_hang, Language.dong);
            else if (viewModel.ContactModel.SelectGroup == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_chon_nhom_khach_hang, Language.dong);
            else if (viewModel.ContactModel.Phone == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_nhap_so_dien_thoai, Language.dong);
            else
            {
                //Contact contact = new Contact();
                //contact.Id = viewModel.ContactModel.Id;
                //contact.FullName = viewModel.ContactModel.FullName;
                //contact.Phone = viewModel.ContactModel.Phone;
                //contact.GroupId = viewModel.ContactModel.GroupId;
                //contact.ProvinceId = viewModel.ContactModel.ProvinceId;
                //contact.DistrictId = viewModel.ContactModel.DistrictId;
                //contact.WardId = viewModel.ContactModel.WardId;
                //contact.Street = viewModel.ContactModel.Street;
                //contact.Address = viewModel.ContactModel.Address;
                //contact.CreatedById = viewModel.ContactModel.CreatedById;
                //contact.CompanyId = viewModel.ContactModel.CompanyById;

                if (viewModel.ContactModel.Province != null)
                {
                    viewModel.ContactModel.ProvinceId = viewModel.ContactModel.Province.Id;
                }
                else viewModel.ContactModel.ProvinceId = null;

                if (viewModel.ContactModel.District != null)
                {
                    viewModel.ContactModel.DistrictId = viewModel.ContactModel.District.Id;
                }
                else viewModel.ContactModel.DistrictId = null;

                if (viewModel.ContactModel.Ward != null)
                {
                    viewModel.ContactModel.WardId = viewModel.ContactModel.Ward.Id;
                }
                else viewModel.ContactModel.WardId = null;

                if (viewModel.ContactModel.SelectGroup != null)
                {
                    viewModel.ContactModel.GroupId = viewModel.ContactModel.SelectGroup.Id;
                }
                loadingPopup.IsVisible = true;
                ApiResponse apiResponce = await ApiHelper.Put(ApiRouter.CONTACT_PUT, viewModel.ContactModel);
                if (apiResponce.IsSuccess)
                {
                    viewModel.Contact = new ContactModel(viewModel.ContactModel);
                    MessagingCenter.Send<ContactDetailContent>(this, "ReloadData");
                    await ModalEditContact.Hide();
                    loadingPopup.IsVisible = false;
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_thanh_cong);
                    
                }
                else await Shell.Current.DisplayAlert(Language.thong_bao, Language.cap_nhat_thong_tin_khach_hang_that_bai, Language.dong);
                loadingPopup.IsVisible = false;
            }
        }
        public async void OnBtnCancelContactDetail_Click(object sender, EventArgs e)
        {
            await ModalEditContact.Hide();
        }
        private void SetFloatingButtonGroup()
        {
            viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, Call_Clicked));
            viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessage_Clicked));
            viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chinh_sua, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProduct));
            viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xoa, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2ed", null, OnDeleteProduct));
        }
        public async void OnEditProduct(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (viewModel.ProvinceList.Count != 0)
            {
                await InitUpdate();
            }

            viewModel.ContactModel = new ContactModel(viewModel.Contact);
            loadingPopup.IsVisible = false;
            await ModalEditContact.Show();
        }
        private async void Call_Clicked(object sender, EventArgs e)
        {
            try
            {
                PhoneDialer.Open(viewModel.Contact.Phone);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        

        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.Contact.Phone));
            }
            catch
            {
                await Shell.Current.DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }
        private async void OnDeleteProduct(object sender, EventArgs e)
        {
            var answer = await Shell.Current.DisplayAlert(Language.xac_nhan_xoa, Language.luu_y_ban_co_chac_chan_muon_xoa_khach_hang_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.CONTACT_DELETE + "/" + viewModel.Contact.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                MessagingCenter.Send<ContactDetailContent, Guid>(this, "DeleteContact", viewModel.Contact.Id);
                await Shell.Current.Navigation.PopAsync();
                ToastMessageHelper.ShortMessage(Language.xoa_khach_hang_thanh_cong);
            }
            else ToastMessageHelper.ShortMessage(Language.xoa_khach_hang_that_bai);

        }
    }
}
