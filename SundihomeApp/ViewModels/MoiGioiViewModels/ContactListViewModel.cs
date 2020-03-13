using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MongoDB.Driver;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class ContactListViewModel : ListViewPageViewModel2<Contact>
    {
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        private List<ContactGroupModel> _groupList;
        public List<ContactGroupModel> GroupList
        {
            get => _groupList;
            set
            {
                _groupList = value;
                OnPropertyChanged(nameof(GroupList));
            }
        }
        public ObservableCollection<Contact> ContactList { get; set; }
        public int PageContact { get; set; } = 1;

        public Guid? CompanyId { get; set; }
        public Guid? CreatedById { get; set; }
        public int GroupId { get; set; } = -1;

        private Boolean _showMoreContact;
        public Boolean ShowMoreContact
        {
            get => _showMoreContact;
            set
            {
                _showMoreContact = value;
                OnPropertyChanged(nameof(ShowMoreContact));
            }
        }
        private ContactModel _contactModel;
        public ContactModel ContactModel
        {
            get => _contactModel;
            set
            {
                _contactModel = value;
                OnPropertyChanged(nameof(ContactModel));
            }
        }
        private ContactGroupModel _selectGroup;
        public ContactGroupModel SelectGroup
        {
            get => _selectGroup;
            set
            {
                _selectGroup = value;
                OnPropertyChanged(nameof(SelectGroup));
            }
        }
        private string _keyword;
        public string Keyword
        {
            get => _keyword;
            set
            {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }
        public ContactListViewModel()
        {
            GroupId = -1;
            ContactList = new ObservableCollection<Contact>();
            ContactModel = new ContactModel();

            PreLoadData = new Command(() =>
                {
                    string url;
                    if (!string.IsNullOrEmpty(this.Keyword))
                        url = ApiRouter.COMPANY_GET_CONTACT + $"?page={this.Page}&GroupId={this.GroupId}&Keyword={this.Keyword}";
                    else url = ApiRouter.COMPANY_GET_CONTACT + $"?page={this.Page}&GroupId={this.GroupId}";

                    if (this.CompanyId.HasValue)
                    {
                        url += $"&CompanyId={this.CompanyId.Value}";
                    }
                    if (this.CreatedById.HasValue)
                    {
                        url += $"&CreatedById={this.CreatedById.Value}";
                    }
                    ApiUrl = url;
                });
        }
        //get list provice
        public async Task GetProviceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            List<Province> data = (List<Province>)apiResponse.Content;
            foreach (var item in data)
            {
                ProvinceList.Add(item);
            }
        }

        //get list district
        public async Task GetDistrictAsync()
        {
            this.DistrictList.Clear();
            if (ContactModel.ProvinceId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{ContactModel.ProvinceId}", false, false);
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
            if (ContactModel.DistrictId.HasValue)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{ContactModel.DistrictId}", false, false);
                List<Ward> data = (List<Ward>)apiResponse.Content;
                foreach (var item in data)
                {
                    WardList.Add(item);
                }
            }
        }
        public void CancelPopUpAddContact()
        {
            ContactModel = new ContactModel();
            SelectGroup = null;
        }
        //get list contact cua cong ty
        public async Task GetContactCongTyAsync()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<Contact>>(ApiRouter.COMPANY_GET_CONTACT + "/" + UserLogged.CompanyId + "?page=" + PageContact, true);
            List<Contact> data = (List<Contact>)apiResponse.Content;
            foreach (var item in data)
            {
                ContactList.Add(item);
            }
        }

        public async Task<bool> PostContactCongTyAsync()
        {
            Contact contact = new Contact();
            contact.FullName = ContactModel.FullName;
            contact.Phone = ContactModel.Phone;
            contact.ProvinceId = ContactModel.Province?.Id;
            contact.DistrictId = ContactModel.District?.Id;
            contact.WardId = ContactModel.Ward?.Id;
            contact.Street = ContactModel.Street;
            contact.Address = ContactModel.Address;
            contact.GroupId = SelectGroup.Id;
            if (CompanyId != null) contact.CompanyId = Guid.Parse(UserLogged.CompanyId);
            ApiResponse apiResponse = await ApiHelper.Post(ApiRouter.CONTACT_ADD, contact);
            List<Contact> data = (List<Contact>)apiResponse.Content;
            if (apiResponse.IsSuccess)
            {
                CancelPopUpAddContact();
                await Shell.Current.DisplayAlert("", Language.them_khach_hang_thanh_cong, Language.dong);
                return true;
            }
            else
            {
                await Shell.Current.DisplayAlert("", apiResponse.Message, Language.dong);
                return false;
            }

        }
        public async Task DeleteContactAsync(Contact item)
        {
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.CONTACT_DELETE + "/" + item.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                Data.Remove(item);
                await Shell.Current.DisplayAlert("", Language.xoa_khach_hang_thanh_cong, Language.dong);
            }
            else await Shell.Current.DisplayAlert("", Language.xoa_khach_hang_thanh_cong, Language.dong);
        }


    }
}
