using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class DangKyMoiGioiContentView : ContentView
    {
        public event EventHandler OnSaved;
        public event EventHandler OnCancel;
        public event EventHandler OnUpdate;
        public bool IsUpdate = false;

        public DangKyMoiGioiContentViewViewModel viewModel;
        public DangKyMoiGioiContentView(BottomModal bottomModal, Guid moiGioiId)
        {
            InitializeComponent();
            BindingContext = viewModel = new DangKyMoiGioiContentViewViewModel();
            lookupType.BottomModal = lookupProvince.BottomModal = lookupDistrict.BottomModal = bottomModal;
            Init(moiGioiId);
        }

        public async void Init(Guid moiGioiId)
        {
            await viewModel.GetProvinceAsync();
            viewModel.MoiGioiModel.Id = moiGioiId;
            this.btnRegister.Clicked += Register_Clicked;
        }

        public async void InitUpdate(MoiGioi moigioi)
        {
            IsUpdate = true;
            var model = new MoiGioiModel();
            model.Id = moigioi.Id;
            model.Introduction = moigioi.Introduction;
            model.StartYear = moigioi.StartYear;
            model.TypeId = moigioi.Type;
            model.Province = moigioi.Province;
            model.ProvinceId = moigioi.ProvinceId;
            model.District = moigioi.District;
            model.DistrictId = moigioi.DistrictId;
            if (moigioi.Type.HasValue)
            {
                model.Type = viewModel.TypeList[moigioi.Type.Value];
            }

            viewModel.MoiGioiModel = model;

            await viewModel.GetDistrictAsync();

            btnRegister.Text = Language.cap_nhat;
            this.Intro.IsVisible = true;
        }

        public void Type_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.MoiGioiModel.Type != null)
            {
                viewModel.MoiGioiModel.TypeId = viewModel.MoiGioiModel.Type.Id;
            }
            else
            {
                viewModel.MoiGioiModel.TypeId = null;
            }
        }

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.MoiGioiModel.Province != null)
            {
                viewModel.MoiGioiModel.ProvinceId = viewModel.MoiGioiModel.Province.Id;
                await viewModel.GetDistrictAsync();
            }
            else
            {
                viewModel.MoiGioiModel.ProvinceId = null;
                viewModel.DistrictList.Clear();
            }
            viewModel.MoiGioiModel.District = null;
            viewModel.MoiGioiModel.DistrictId = null;
            lookupDistrict.SelectedItem = null;
        }

        public void District_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.MoiGioiModel.District != null)
            {
                viewModel.MoiGioiModel.DistrictId = viewModel.MoiGioiModel.District.Id;
            }
            else
            {
                viewModel.MoiGioiModel.DistrictId = null;
            }
        }

        public void Clear()
        {
            ExpPicker.SelectedItem = null;
            lookupType.SelectedItem = null;
            lookupProvince.SelectedItem = null;
            lookupDistrict.SelectedItem = null;
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            await DangKyMoiGioi();
        }
        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            bool answer = await Shell.Current.DisplayAlert("", Language.ban_co_chac_chan_muon_huy_khong, Language.huy, Language.tiep_tuc);
            if (answer)
            {
                Clear();
                OnCancel?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task DangKyMoiGioi()
        {
            if (!viewModel.MoiGioiModel.StartYear.HasValue)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_kinh_nghiem, Language.dong);
                return;
            }
            if (!viewModel.MoiGioiModel.TypeId.HasValue)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_loai_hinh_moi_gioi, Language.dong);
                return;
            }
            if (!viewModel.MoiGioiModel.DistrictId.HasValue || !viewModel.MoiGioiModel.ProvinceId.HasValue)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_khu_vuc_hoat_dong, Language.dong);
                return;
            }

            viewModel.MoiGioi.Id = viewModel.MoiGioiModel.Id;
            viewModel.MoiGioi.Introduction = viewModel.MoiGioiModel.Introduction;
            viewModel.MoiGioi.StartYear = viewModel.MoiGioiModel.StartYear.Value;
            viewModel.MoiGioi.Type = viewModel.MoiGioiModel.TypeId.Value;
            viewModel.MoiGioi.ProvinceId = viewModel.MoiGioiModel.ProvinceId.Value;
            viewModel.MoiGioi.DistrictId = viewModel.MoiGioiModel.DistrictId.Value;
            viewModel.MoiGioi.Address = viewModel.MoiGioiModel.District.Name + ", " + viewModel.MoiGioiModel.Province.Name;

            ApiResponse response = null;
            if (IsUpdate == false)
            {
                response = await ApiHelper.Post(ApiRouter.MOIGIOI_REGISTER, viewModel.MoiGioi, true);
            }
            else
            {
                response = await ApiHelper.Put(ApiRouter.MOIGIOI_GETBYID, viewModel.MoiGioi, true);
            }

            if (response.IsSuccess)
            {
                // set null.
                viewModel.MoiGioi.Province = viewModel.MoiGioiModel.Province;
                viewModel.MoiGioi.District = viewModel.MoiGioiModel.District;

                if (!IsUpdate)
                {
                    UserLogged.Type = 1;
                    await Shell.Current.DisplayAlert("", Language.dang_ky_moi_gioi_thanh_cong, Language.dong);

                    // them menu.
                    var appShell = (AppShell)Shell.Current;
                    appShell.AddMenu_QuanLyMoiGioi();
                }
                else
                {
                    await Shell.Current.DisplayAlert("", Language.cap_nhat_moi_gioi_thanh_cong, Language.dong);
                }
                OnSaved?.Invoke(this, null);
            }
            else
            {
                await Shell.Current.DisplayAlert("", response.Message, Language.dong);
            }
        }
    }
}
