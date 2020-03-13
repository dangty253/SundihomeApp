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
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class AddContactNeedContentViewModel : ListViewPageViewModel2<ContactNeed>
    {
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();

        private List<LoaiBatDongSanModel> _loaiBatDongSanList;
        public List<LoaiBatDongSanModel> LoaiBatDongSanList
        {
            get => _loaiBatDongSanList;
            set
            {
                _loaiBatDongSanList = value;
                OnPropertyChanged(nameof(LoaiBatDongSanList));
            }
        }
        private ContactNeedModel _contactNeedModel;
        public ContactNeedModel ContactNeedModel
        {
            get => _contactNeedModel;
            set
            {
                _contactNeedModel = value;
                OnPropertyChanged(nameof(ContactNeedModel));
            }
        }
        public Contact _contact { get; set; }

        private string _method;
        public string Method
        {
            get => _method;
            set
            {
                _method = value;
                OnPropertyChanged(nameof(Method));
            }
        }
        
        private LoaiBatDongSanModel _loaiBatDongSanSelected;
        public LoaiBatDongSanModel LoaiBatDongSanSelected
        {
            get => _loaiBatDongSanSelected;
            set
            {
                _loaiBatDongSanSelected = value;
                OnPropertyChanged(nameof(LoaiBatDongSanSelected));
            }
        }

        public AddContactNeedContentViewModel()
        {

            ContactNeedModel = new ContactNeedModel();
            PreLoadData = new Command(() =>
            {
                ApiUrl = ApiRouter.CONTACT_GET_NEEDS + "/" + _contact.Id + "?page=" + Page;
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
            ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{ContactNeedModel.ProvinceId}", false, false);
            List<District> data = (List<District>)apiResponse.Content;
            foreach (var item in data)
            {
                DistrictList.Add(item);
            }

        }
        public void CancelPopUpAddContactNeed()
        {
            ContactNeedModel = new ContactNeedModel();
            DistrictList.Clear();
            LoaiBatDongSanSelected = null;
        }

        public async Task DeleteContactNeedAsync(ContactNeed item)
        {
            ApiResponse apiResponse = await ApiHelper.Delete(ApiRouter.CONTACT_DELETE_NEED + "/" + item.Id.ToString(), true);
            if (apiResponse.IsSuccess)
            {
                Data.Remove(item);
                ToastMessageHelper.ShortMessage(Language.xoa_nhu_cau_thanh_cong);
                MessagingCenter.Send<AddContactNeedContentViewModel>(this, "ReloadNhuCauList");
                MessagingCenter.Send<AddContactNeedContentViewModel, ContactNeed>(this, "DeleteContactNeed", item);
            }
            else await Shell.Current.DisplayAlert("", Language.xoa_nhu_cau_that_bai, Language.dong);
        }
    }
}
