using System;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels.BankViewModel;
using Xamarin.Forms;

namespace SundihomeApp.Views.BankViews
{
    public partial class DangKyNhanVienNganHangView : ContentView
    {
        public event EventHandler OnCancel;
        public event EventHandler OnSaved;
        private readonly DangKyNhanVienNganHangViewModel viewModel;
        public DangKyNhanVienNganHangView(BottomModal bottomModal)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new DangKyNhanVienNganHangViewModel();
            lookupBank.BottomModal = lookupProvince.BottomModal = lookupDistrict.BottomModal = bottomModal;
            Init();
            InitAdd();
        }

        public DangKyNhanVienNganHangView(BottomModal bottomModal, Guid id)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new DangKyNhanVienNganHangViewModel();
            lookupBank.BottomModal = lookupProvince.BottomModal = lookupDistrict.BottomModal = bottomModal;
            viewModel.Id = id;
            Init();
            InitUpdate();
        }

        public async void Init()
        {
            btnRegister.Clicked += BtnRegister_Clicked;
            loadingPopup.IsVisible = false;
        }
        public async void InitAdd()
        {
            await Task.WhenAll(viewModel.LoadBanks(), viewModel.GetProvinceAsync());
        }

        public async void InitUpdate()
        {
            btnRegister.Text = Language.luu;
            var response = await ApiHelper.Get<BankEmployee>(ApiRouter.BANK_EMPLOYEE_DETAIL + viewModel.Id);
            if (response.IsSuccess)
            {
                await Task.WhenAll(viewModel.LoadBanks(), viewModel.GetProvinceAsync());

                BankEmployee emp = response.Content as BankEmployee;
                viewModel.Bank = emp.Bank;
                viewModel.Province = this.viewModel.ProvinceList.SingleOrDefault(x => x.Id == emp.ProvinceId);
                await viewModel.GetDistrictAsync();
                if (emp.DistrictId.HasValue)
                {
                    viewModel.District = this.viewModel.DistrictList.SingleOrDefault(x => x.Id == emp.DistrictId.Value);
                }
            }
            loadingPopup.IsVisible = false;
        }

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Bank == null)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_ngan_hang, Language.dong);
                return;
            }
            if (viewModel.Province == null)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_chon_tinh_thanh, Language.dong);
                return;
            }
            loadingPopup.IsVisible = true;

            BankEmployee bankEmployee = new BankEmployee();
            bankEmployee.BankId = viewModel.Bank.Id;
            bankEmployee.ProvinceId = viewModel.Province.Id;
            if (viewModel.District != null)
            {
                bankEmployee.DistrictId = viewModel.District.Id;
                bankEmployee.Address = viewModel.District.Name + ", " + viewModel.Province.Name;
            }
            else
            {
                bankEmployee.Address = viewModel.Province.Name;
            }

            ApiResponse apiResponse = null;
            if (viewModel.Id == Guid.Empty)
            {
                apiResponse = await ApiHelper.Post(ApiRouter.BANK_EMPLOYEE_REGISTER, bankEmployee, true);
            }
            else
            {
                bankEmployee.Id = viewModel.Id;
                apiResponse = await ApiHelper.Put(ApiRouter.BANK_EMPLOYEE_UPDATE, bankEmployee, true);
            }

            if (apiResponse.IsSuccess)
            {
                OnSaved?.Invoke(this, EventArgs.Empty);
                loadingPopup.IsVisible = false;
            }
            else
            {
                loadingPopup.IsVisible = false;
                await Shell.Current.DisplayAlert("", apiResponse.Message, Language.dong);
            }
        }

        private async void Province_Change(object sender, LookUpChangeEvent e)
        {
            await viewModel.GetDistrictAsync();
        }
        private void Cancel_Clicked(object sender, EventArgs e)
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
