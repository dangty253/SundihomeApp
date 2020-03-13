using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SundihomeApp.Views
{
    public partial class AddEmployeePage : ContentPage
    {
        public AddEmployeePageViewModel viewModel;
        public Guid _id;
        public AddEmployeePage(Guid Id)
        {
            InitializeComponent();
            _id = Id;
            Init();
            InitUpdate();
        }
        public AddEmployeePage()
        {
            InitializeComponent();
            Init();
            InitAdd();
        }

        public async void Init()
        {
            On<iOS>().SetUseSafeArea(true);
            this.BindingContext = viewModel = new AddEmployeePageViewModel();
        }

        public async void InitAdd()
        {
            viewModel.Title = Language.them_nhan_vien;
            //btnAddEmployee.IsVisible = true;
            await viewModel.GetProvinceAsync();
            loadingPopup.IsVisible = false;
        }

        public async void InitUpdate()
        {
            viewModel.Title = Language.cap_nhat_nhan_vien;
            //btnAddEmployee.Text = Language.luu_nhan_vien;

            ApiResponse apiResponse = await ApiHelper.Get<EmployeeModel>("api/user/" + this._id);
            if (!apiResponse.IsSuccess) return;

            var model = apiResponse.Content as EmployeeModel;
            viewModel.EmployeeModel = model;


            //set lai tinh thang, quan huyen, phuong xa
            await viewModel.GetProvinceAsync();
            if (model.ProvinceId.HasValue)
            {
                viewModel.EmployeeModel.Province = viewModel.ProvinceList.Single(x => x.Id == model.ProvinceId);
            }
            else viewModel.EmployeeModel.Province = null;

            if (model.DistrictId.HasValue)
            {
                await viewModel.GetDistrictAsync();
                viewModel.EmployeeModel.District = viewModel.DistrictList.Single(x => x.Id == model.DistrictId);
            }
            else viewModel.EmployeeModel.District = null;

            if (model.WardId.HasValue)
            {
                await viewModel.GetWardAsync();
                viewModel.EmployeeModel.Ward = viewModel.WardList.Single(x => x.Id == model.WardId);
            }
            else viewModel.EmployeeModel.Ward = null;


            loadingPopup.IsVisible = false;

        }


        public async void SendInviteCode()
        {
            Random random = new Random();
            int ran1 = random.Next(0, 9);
            int ran2 = random.Next(0, 9);
            int ran3 = random.Next(0, 9);
            int ran4 = random.Next(0, 9);

            string opt = $"{ran1}" + $"{ran2}" + $"{ran3}" + $"{ran4}";
            viewModel.InviteUser.InviteCode = opt;
            viewModel.InviteUser.PhoneNumber = viewModel.EmployeeModel.Phone;
            viewModel.InviteUser.Status = 0;
            viewModel.InviteUser.CompanyId = UserLogged.CompanyId;

            ApiResponse response = await ApiHelper.Get<User>($"{ApiRouter.EMPLOYEE_GET_EMPLOYEE_BY_PHONE}/{viewModel.EmployeeModel.Phone}", true);
            User user = new User();
            if (response.Content != null)
            {
                user = response.Content as User;
                viewModel.InviteUser.UserId = user.Id.ToString();
            }
            else
            {
                viewModel.InviteUser.UserId = null;
            }

            ApiResponse apiResponse = await ApiHelper.Post("api/company/InviteUser", viewModel.InviteUser, true);
            if (apiResponse.IsSuccess)
            {
                await DisplayAlert(Language.thong_bao, Language.gui_ma_thanh_cong, Language.dong);
                MessagingCenter.Send<AddEmployeePage>(this, "OnSendInViteUser");
                await Shell.Current.Navigation.PopAsync();
            }
        }

        public async void OnClick_AddEmployee(object sender, EventArgs e)
        {
            if (!Validations.IsValidPhone(viewModel.EmployeeModel.Phone))
            {
                await DisplayAlert(Language.thong_bao, Language.so_dien_thoai_khong_hop_le_vui_long_kiem_tra_lai, Language.dong);
                return;
            }
            if (viewModel.EmployeeModel.FullName == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_ho_ten, Language.dong);
                return;
            }
            if (!Validations.IsValidEmail(viewModel.EmployeeModel.Email))
            {
                await DisplayAlert(Language.thong_bao, Language.email_khong_hop_le, Language.dong);
                return;
            }

            if (viewModel.EmployeeModel.Province == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_tinh_thanh, Language.dong);
                return;
            }
            if (viewModel.EmployeeModel.District == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_quan_huyen, Language.dong);
                return;
            }
            if (viewModel.EmployeeModel.Ward == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_phuong_xa, Language.dong);
                return;
            }

            viewModel.EmployeeModel.RoleId = null;

            viewModel.EmployeeModel.RegisterDate = DateTime.Now;

            //get provinceId, disctrictId, WardId
            if (viewModel.EmployeeModel.Province != null)
            {
                viewModel.EmployeeModel.ProvinceId = viewModel.EmployeeModel.Province.Id;
            }
            else
            {
                viewModel.EmployeeModel.ProvinceId = null;
            }
            if (viewModel.EmployeeModel.District != null)
            {
                viewModel.EmployeeModel.DistrictId = viewModel.EmployeeModel.District.Id;
            }
            else
            {
                viewModel.EmployeeModel.DistrictId = null;
            }
            if (viewModel.EmployeeModel.Ward != null)
            {
                viewModel.EmployeeModel.WardId = viewModel.EmployeeModel.Ward.Id;
            }
            else
            {
                viewModel.EmployeeModel.WardId = null;
            }

            loadingPopup.IsVisible = true;
            if (viewModel.EmployeeModel.Phone != null)
            {
                if (this._id == Guid.Empty)
                {
                    ApiResponse apiresponse = await ApiHelper.Get<EmployeeModel>($"{ApiRouter.EMPLOYEE_GET_EMPLOYEE_BY_PHONE}/{viewModel.EmployeeModel.Phone}", true);

                    if (apiresponse.Content == null) // kiem tra ton tai so dien thoai
                    {
                        viewModel.EmployeeModel.CompanyId = Guid.Parse(UserLogged.CompanyId);
                        viewModel.EmployeeModel.Id = Guid.NewGuid();
                        ApiResponse response = await ApiHelper.Post("api/employee", viewModel.EmployeeModel, false);
                        if (response.IsSuccess)
                        {
                            loadingPopup.IsVisible = false;
                            await DisplayAlert(Language.thong_bao, Language.them_nhan_vien_thanh_cong, Language.dong);
                            MessagingCenter.Send<AddEmployeePage>(this, "OnSaveEmployee");
                            await Shell.Current.Navigation.PopAsync();
                        }
                        return;
                    }

                    var model = apiresponse.Content as EmployeeModel;
                    if (model.CompanyId.HasValue) //co cong ty
                    {
                        //kiem tra da ton tai trong cong ty hien tai neu co
                        if (model.CompanyId == Guid.Parse(UserLogged.CompanyId))
                        {
                            loadingPopup.IsVisible = false;
                            await DisplayAlert(Language.thong_bao, Language.nhan_vien_da_co_trong_cong_ty, Language.dong);
                            return;
                        }
                    }

                    bool answer = await DisplayAlert(Language.xac_nhan, Language.so_dien_thoai_nay_da_co_tren_he_thong_ban_co_muon_gui_ma_moi_toi_so_dien_thoai_nay_khong, Language.co, Language.khong);
                    loadingPopup.IsVisible = true;
                    if (answer)
                    {
                        ICompanyService companyService = DependencyService.Get<ICompanyService>();
                        List<InviteUser> inviteUsers = companyService.GetListInvitedUser(viewModel.EmployeeModel.Phone, UserLogged.CompanyId);
                        // xu ly
                        foreach (var item in inviteUsers) // xoa user da gui ma opt
                        {
                            companyService.DeleteInvitedUser(item.Id);
                        }
                        SendInviteCode();
                    }
                    loadingPopup.IsVisible = false;
                }
                else
                {
                    if (UserLogged.Id == viewModel.EmployeeModel.Id.ToString() || viewModel.EmployeeModel.Id == viewModel.EmployeeModel.Company.CreatedById)
                    {
                        await DisplayAlert(Language.thong_bao, Language.ban_khong_the_cap_nhat, Language.dong);
                        loadingPopup.IsVisible = false;
                        return;
                    }
                    ApiResponse apiResponse = await ApiHelper.Put("api/employee/UpdateUser", viewModel.EmployeeModel, true);
                    if (apiResponse.IsSuccess)
                    {
                        await DisplayAlert(Language.thong_bao, Language.cap_nhat_nhan_vien_thanh_cong, Language.dong);
                        MessagingCenter.Send<AddEmployeePage>(this, "OnSaveEmployee");
                        await Shell.Current.Navigation.PopAsync();
                    }
                }
            }
            loadingPopup.IsVisible = false;
        }

        public async void Province_change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.EmployeeModel.Province != null)
            {
                viewModel.EmployeeModel.ProvinceId = viewModel.EmployeeModel.Province.Id;
            }
            else
            {
                viewModel.EmployeeModel.ProvinceId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.EmployeeModel.District = null;
            viewModel.EmployeeModel.Ward = null;
            viewModel.WardList.Clear();
            loadingPopup.IsVisible = false;
        }

        public async void District_change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.EmployeeModel.District != null)
            {
                viewModel.EmployeeModel.DistrictId = viewModel.EmployeeModel.District.Id;
            }
            else
            {
                viewModel.EmployeeModel.DistrictId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.EmployeeModel.Ward = null;
            loadingPopup.IsVisible = false;
        }
    }
}
