using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class ContactListPage : ContentPage
    {
        public ContactListViewModel viewModel;
        public ListViewPageViewModel2<ContactNeed> viewModelContactNeed;
        private bool _isCompany;
        private ContactNeedFilterModel contactNeedFilterModel;
        public ContactListPage(bool IsCompany)
        {
            InitializeComponent();
            _isCompany = IsCompany;
            this.BindingContext = viewModel = new ContactListViewModel();
            viewModel.CompanyId = Guid.Parse(UserLogged.CompanyId);
            ModalAddContact.Body.BindingContext = viewModel;
            Init();
        }

        public ContactListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ContactListViewModel();
            viewModel.CreatedById = Guid.Parse(UserLogged.Id);
            ModalAddContact.Body.BindingContext = viewModel;
            Init();
        }

        public async void Init()
        {
            segment.ItemsSource = new List<string> {Language.nhu_cau,Language.moi,Language.dau_tu,Language.da_mua };
            segment.SetActive(0);
            LvData.ItemTapped += LvData_ItemTapped;
            MessagingCenter.Subscribe<ContactDetailContent>(this, "ReloadData", async (sender) =>
              {
                  loadingPopup.IsVisible = true;
                  await viewModel.LoadOnRefreshCommandAsync();
                  loadingPopup.IsVisible = false;
              });
            MessagingCenter.Subscribe<ContactDetailContent, Guid>(this, "DeleteContact", async (sender, ContactId) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModel.Data.Any(x => x.Id == ContactId))
                {
                    await viewModel.LoadOnRefreshCommandAsync();
                }
                await viewModelContactNeed.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<AddContactNeedContent>(this, "AddContactNeed", async (sender) =>
            {
                loadingPopup.IsVisible = true;
                await viewModelContactNeed.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<AddContactNeedContent, Guid>(this, "UpdateContactNeed", async (sender, ContactNeedId) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModelContactNeed.Data.Any(x => x.Id == ContactNeedId))
                {
                    await viewModelContactNeed.LoadOnRefreshCommandAsync();
                }
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<AddContactNeedContent, ContactNeed>(this, "DeleteContactNeed", async (sender, ContactNeed) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModelContactNeed.Data.Any(x => x.Id == ContactNeed.Id))
                {
                    viewModelContactNeed.Data.Remove(ContactNeed);
                }
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<AddContactNeedContentViewModel, ContactNeed>(this, "DeleteContactNeed", async (sender, ContactNeed) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModelContactNeed.Data.Any(x => x.Id == ContactNeed.Id))
                {
                    viewModelContactNeed.Data.Remove(ContactNeed);
                }
                loadingPopup.IsVisible = false;
            }); 
                
            viewModel.GroupList = Models.ContactGroupModel.GetList();
            ModalAddContact.CustomCloseButton(OnBtnCancelAddContact_Click);

            contactNeedFilterModel = new ContactNeedFilterModel();
            if (viewModel.CompanyId.HasValue)
            {
                contactNeedFilterModel.CompanyId = viewModel.CompanyId.Value;
            }
            if (viewModel.CreatedById.HasValue)
            {
                contactNeedFilterModel.CreatedById = viewModel.CreatedById.Value;
            }
            if (viewModel.Keyword!=null)
            {
                contactNeedFilterModel.Keyword = viewModel.Keyword;
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(contactNeedFilterModel);
            viewModelContactNeed = new ListViewPageViewModel2<ContactNeed>()
            {
                PreLoadData = new Command(() =>
                {
                    string url;
                        contactNeedFilterModel.Keyword = viewModel.Keyword;
                    url = ApiRouter.COMPANY_GET_FILTER_CONTACTNEEDS + $"?json={json}" + $"&page={viewModelContactNeed.Page}";
                    
                    
                    viewModelContactNeed.ApiUrl = url;
                })
            };
            LvDataContactNeed.BindingContext = viewModelContactNeed;
            LvDataContactNeed.ItemTapped += LvDataContactNeed_ItemTapped;
            await viewModelContactNeed.LoadData();

            loadingPopup.IsVisible = false;
        }
        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var contact = e.Item as Contact;
            await Navigation.PushAsync(new Views.MoiGioiViews.ContactDetailPage(contact.Id, _isCompany));
        }
        private async void LvDataContactNeed_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var contact = e.Item as ContactNeed;
            await Navigation.PushAsync(new Views.MoiGioiViews.ContactDetailPage(contact.Contact.Id, _isCompany));
        }
        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            //chon tinh/thanh
            if (viewModel.ContactModel.Province != null)
            {
                viewModel.ContactModel.ProvinceId = viewModel.ContactModel.Province.Id;
            }
            else
            {
                viewModel.ContactModel.ProvinceId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
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
            }
            else
            {
                viewModel.ContactModel.DistrictId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.ContactModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void AddNewContact_Clicked(Object Sender, EventArgs e)
        {
            if (viewModel.SelectGroup != null || viewModel.GroupId != -1)
            {
                viewModel.SelectGroup = Models.ContactGroupModel.GetList().Single(x => x.Id == viewModel.GroupId);
            }
            if (viewModel.ProvinceList.Count == 0)
            {
                await viewModel.GetProviceAsync();
            }
            await viewModel.GetDistrictAsync();
            await viewModel.GetWardAsync();
            await ModalAddContact.Show();
        }

        public async void OnBtnSaveAddContact_Click(object sender, EventArgs e)
        {
            if (viewModel.ContactModel.FullName == null) await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_ten_khach_hang, Language.dong);
            else if (viewModel.SelectGroup == null) await DisplayAlert(Language.thong_bao, Language.vui_long_chon_nhom_khach_hang, Language.dong);
            else if (viewModel.ContactModel.Phone == null) await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_so_dien_thoai, Language.dong);
            else
            {
                Contact contact = new Contact();
                contact.FullName = viewModel.ContactModel.FullName;
                contact.Phone = viewModel.ContactModel.Phone;
                contact.ProvinceId = viewModel.ContactModel.Province?.Id;
                contact.DistrictId = viewModel.ContactModel.District?.Id;
                contact.WardId = viewModel.ContactModel.Ward?.Id;
                contact.Street = viewModel.ContactModel.Street;
                contact.Address = viewModel.ContactModel.Address;
                contact.GroupId = viewModel.SelectGroup.Id;
                if (viewModel.CompanyId != null) contact.CompanyId = Guid.Parse(UserLogged.CompanyId);
                ApiResponse apiResponse = await ApiHelper.Post(ApiRouter.CONTACT_ADD, contact);
                if (apiResponse.IsSuccess)
                {
                    await ModalAddContact.Hide();
                    viewModel.CancelPopUpAddContact();
                    await viewModel.LoadOnRefreshCommandAsync();
                    ToastMessageHelper.ShortMessage(Language.them_khach_hang_thanh_cong);

                }
                else
                {
                    ToastMessageHelper.ShortMessage(apiResponse.Message);
                }
            }
        }

        public async void OnBtnCancelAddContact_Click(object sender, EventArgs e)
        {
            viewModel.CancelPopUpAddContact();
            await ModalAddContact.Hide();
        }
        public async void DeleteContactItemClick(object sender, EventArgs e)
        {
            var answer = await Shell.Current.DisplayAlert(Language.xac_nhan_xoa, Language.luu_y_ban_co_chac_chan_muon_xoa_khach_hang_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            Contact item = ((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as Contact;
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.CONTACT_DELETE + "/" + item.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                viewModel.Data.Remove(item);
                ToastMessageHelper.ShortMessage(Language.xoa_khach_hang_thanh_cong);
            }
            else ToastMessageHelper.ShortMessage(Language.xoa_khach_hang_that_bai);
        }
        public void SegmentSelected_Tapped(Object sender, EventArgs e)
        {

            int status = segment.GetCurrentIndex() - 1;

            if (status != -1) viewModel.SelectGroup = Models.ContactGroupModel.GetList().Single(x => x.Id == status);
            else viewModel.SelectGroup = null;
            viewModel.GroupId = status;
            if (status == -1)
            {
                LvDataContactNeed.IsVisible = true;
                LvData.IsVisible = false;
                ButtonAdd.IsVisible = false;
            }
            else
            {
                LvDataContactNeed.IsVisible = false;
                LvData.IsVisible = true;
                ButtonAdd.IsVisible = true;
                viewModel.RefreshCommand.Execute(null);

            }
        }
        public async void Search_Clicked(Object sender, EventArgs e)
        {
            viewModel.Keyword = searchBar.Text;
            loadingPopup.IsVisible = true;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }
        private void SearchText_Changed(object sender, TextChangedEventArgs e)
        {
            if ((e.NewTextValue == null || e.NewTextValue == "") && !string.IsNullOrWhiteSpace(this.viewModel.Keyword))
            {
                Search_Clicked(null, EventArgs.Empty);
            }
        }
        public async void DeleteContactNeedItemClick(object sender, EventArgs e)
        {
            var answer = await Shell.Current.DisplayAlert(Language.xac_nhan_xoa, Language.luu_y_ban_co_chac_chan_muon_xoa_nhu_cau_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            ContactNeed item = ((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as ContactNeed;
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.CONTACT_DELETE_NEED + "/" + item.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                viewModelContactNeed.Data.Remove(item);
                ToastMessageHelper.ShortMessage(Language.xoa_nhu_cau_thanh_cong);
            }
            else await Shell.Current.DisplayAlert("", Language.xoa_nhu_cau_that_bai, Language.dong);
        }
    }
}
