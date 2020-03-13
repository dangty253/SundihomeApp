using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using SundihomeApp.Views.MoiGioiViews;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public class NhanVienListViewModel : ViewModels.ListViewPageViewModel2<User>
    {
        public string Keyword;
        public NhanVienListViewModel(Guid companyId)
        {
            PreLoadData = new Command(() =>
            {

                string url = $"{ApiRouter.COMPANY_GET_EMPLOYEE}/{companyId}?page={Page}";
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    url += $"&Keyword={Keyword}";
                }
                ApiUrl = url;
            });
        }
    }

    public class InviteListViewModel : ViewModels.ListViewPageViewModel2<InviteUser>
    {
        private InviteUser _inviteUser;
        public InviteUser InviteUser
        {
            get => _inviteUser;
            set
            {
                _inviteUser = value;
                OnPropertyChanged(nameof(InviteUser));
            }
        }

        public InviteListViewModel(Guid companyId)
        {
            InviteUser = new InviteUser();
            IUserService userService = DependencyService.Get<IUserService>();

            OnMapItem = new Command<InviteUser>((UserInvite) =>
            {
                UserInvite.User = userService.Find(UserInvite.UserId);
            });
            PreLoadData = new Command(() => ApiUrl = $"{ApiRouter.COMPANY_GET_INVITE_USER}/{companyId}?page={Page}");
        }
    }

    public partial class EmployeeListPage : ContentPage
    {
        public HoSoDangKyLIstPageViewModel hoSoDangKyLIstPageViewModel;
        public InviteListViewModel inviteListViewModel;
        public NhanVienListViewModel nhanVienListViewModel;
        private Guid _companyId;
        public EmployeeListPage()
        {
            InitializeComponent();
            _companyId = Guid.Parse(UserLogged.CompanyId);
            this.lvDanhSachNhanVien.BindingContext = nhanVienListViewModel = new NhanVienListViewModel(_companyId);
            this.lvInviteUser.BindingContext = inviteListViewModel = new InviteListViewModel(_companyId);
            this.lvChoXetDuyet.BindingContext = hoSoDangKyLIstPageViewModel = new HoSoDangKyLIstPageViewModel(_companyId);

            Init();
        }
        public async void Init()
        {
            segment.ItemsSource = new List<string> { Language.nhan_vien, Language.cho_xac_nhan, Language.cho_xet_duyet };
            segment.SetActive(0);
            MessagingCenter.Subscribe<AddEmployeePage>(this, "OnSaveEmployee", async (sender) =>
            {
                this.nhanVienListViewModel.RefreshCommand.Execute(null);
            });

            lvChoXetDuyet.ItemTapped += LvChoXetDuyet_ItemTapped;
            lvDanhSachNhanVien.ItemTapped += LvDanhSachNhanVien_ItemTapped;
            ModalPopup.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            await nhanVienListViewModel.LoadData();

            loadingPopup.IsVisible = false;
        }

        private void Search_Pressed(object sender, EventArgs e)
        {
            nhanVienListViewModel.Keyword = searchBar.Text;
            nhanVienListViewModel.RefreshCommand.Execute(null);
        }
        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text) && !string.IsNullOrWhiteSpace(nhanVienListViewModel.Keyword))
            {
                nhanVienListViewModel.Keyword = null;
                nhanVienListViewModel.RefreshCommand.Execute(null);
            }
        }

        private async void LvDanhSachNhanVien_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var user = e.Item as User;
            if (user.Type == 1)
            {
                await Navigation.PushAsync(new MoiGioiViews.ThongTinMoiGioiPage(user.Id, false));
            }
            else
            {
                await Navigation.PushAsync(new UserProfilePage(user.Id));
            }
        }

        private async void LvChoXetDuyet_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as SundihomeApi.Entities.CompanyEntities.HoSoDangKyNhanVien;
            string[] option = { Language.xem_thong_tin_moi_gioi, Language.duyet, Language.khong_duyet };
            string action = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, option);

            if (action == option[0])
            {
                await Navigation.PushAsync(new ThongTinMoiGioiPage(item.UserId, false));
            }
            else if (action == option[1])
            {
                var respnose = await ApiHelper.Put(ApiRouter.EMPLOYEE_APPROVE_HOSODANGKY + "/" + item.Id, null, true);
                if (respnose.IsSuccess)
                {
                    await DisplayAlert("", Language.duyet_thanh_cong, Language.dong);
                    hoSoDangKyLIstPageViewModel.Data.Remove(item);

                    // lay thong tin cong ty.
                    var companyReponse = await ApiHelper.Get<Company>("api/company/" + _companyId);
                    if (companyReponse.IsSuccess == false) return;

                    var company = companyReponse.Content as Company;

                    // gui thong bao cho uuesr la dang ky nhan vien thanh cong.

                    INotificationService notificationService = DependencyService.Get<INotificationService>();
                    var notification = new NotificationModel()
                    {
                        UserId = item.UserId,
                        Title = Language.ban_da_tro_thanh_nhan_vien_cong_ty +" " + company.Name,
                        NotificationType = NotificationType.RegisterEmployeeSuccess,
                        CreatedDate = DateTime.Now,
                        IsRead = false,
                    };

                    await notificationService.AddNotification(notification, Language.dang_ky_nhan_vien);


                    // refresh du lieu o 2 .
                    await hoSoDangKyLIstPageViewModel.LoadOnRefreshCommandAsync();

                    // clear de khi tap vap nhan vien thi load lai du lieu.
                    nhanVienListViewModel.Data.Clear();
                }
                else
                {
                    await DisplayAlert("", respnose.Message, Language.dong);
                }
            }
            else if (action == option[2])
            {
                var respnose = await ApiHelper.Put(ApiRouter.EMPLOYEE_REJECT_HOSODANGKY + "/" + item.Id, null, true);
                if (respnose.IsSuccess)
                {
                    await DisplayAlert("", Language.thanh_cong, Language.dong);
                    hoSoDangKyLIstPageViewModel.Data.Remove(item);
                }
                else
                {
                    await DisplayAlert("", respnose.Message, Language.dong);
                }
            }
        }

        private async void SegmentSelected_Tapped(object sender, EventArgs e)
        {
            int index = segment.GetCurrentIndex();

            if (index == 0)
            {
                if (nhanVienListViewModel.Data.Any() == false) // bi clear khi duyet nen kiem tar de load lai du ieu.
                {
                    await nhanVienListViewModel.LoadOnRefreshCommandAsync();
                }
                lvDanhSachNhanVien.IsVisible = true;
                lvInviteUser.IsVisible = false;
                lvChoXetDuyet.IsVisible = false;
            }
            else if (index == 1)
            {
                if (inviteListViewModel.Data.Any() == false)
                {
                    await inviteListViewModel.LoadOnRefreshCommandAsync();
                }
                lvDanhSachNhanVien.IsVisible = false;
                lvInviteUser.IsVisible = true;
                lvChoXetDuyet.IsVisible = false;
            }
            else
            {
                if (hoSoDangKyLIstPageViewModel.Data.Any() == false)
                {
                    await hoSoDangKyLIstPageViewModel.LoadOnRefreshCommandAsync();
                }
                lvDanhSachNhanVien.IsVisible = false;
                lvInviteUser.IsVisible = false;
                lvChoXetDuyet.IsVisible = true;
            }
        }

        public async void UserProfile_Tapped(object sender, ItemTappedEventArgs e)
        {
            User user = (User)e.Item;
            await Shell.Current.Navigation.PushAsync(new UserProfilePage(user.Id));
        }

        private async void AddEmployee_Clicked(object sender, EventArgs e)
        {
            string action = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, Language.them_moi_nhan_vien, Language.gui_ma_moi);
            if (action == Language.them_moi_nhan_vien)
            {
                await Shell.Current.Navigation.PushAsync(new AddEmployeePage());
            }
            if (action == Language.gui_ma_moi)
            {
                ModalPopup.IsVisible = true;
            }

        }

        private async void BtnSendOPT_Clicked(object sender, EventArgs e)
        {

            if (enNumPhone.Text != null)
            {
                if (!Validations.IsValidPhone(enNumPhone.Text))
                {
                    await Shell.Current.DisplayAlert("", Language.so_dien_thoai_khong_dung, Language.dong);
                    return;
                }

                ICompanyService companyService = DependencyService.Get<ICompanyService>();
                List<InviteUser> listInvitedUser = companyService.GetListInvitedUser(enNumPhone.Text, UserLogged.CompanyId);
                bool inviteUser = companyService.FindInvitedUser(enNumPhone.Text, UserLogged.CompanyId);

                ApiResponse apiresponse = await ApiHelper.Get<EmployeeModel>("api/employee/GetUserByPhone/" + enNumPhone.Text, true);

                if (apiresponse.Content == null) // kiem tra so dien thoai (khi khong co sdt)
                {
                    if (inviteUser)
                    {
                        bool answer = await Shell.Current.DisplayAlert(Language.thong_bao, Language.so_dien_thoai_nay_da_gui_ma_opt_ban_co_muon_tiep_tuc, Language.co, Language.khong);
                        if (!answer) return;

                        foreach (var item in listInvitedUser)
                        {
                            companyService.DeleteInvitedUser(item.Id);// xoa user da gui ma opt
                        }
                        SendInviteCode();
                        return;
                    }
                    SendInviteCode();
                    return;
                }
                var model = apiresponse.Content as EmployeeModel;// khi co sdt
                //kiem tra da ton tai trong cong ty hien tai neu co
                if (model.CompanyId == Guid.Parse(UserLogged.CompanyId)) // da co trong cong ty
                {
                    await Shell.Current.DisplayAlert(Language.thong_bao, Language.nhan_vien_da_co_trong_cong_ty, Language.dong);
                    enNumPhone.Text = null;
                    return;
                }

                if (inviteUser)
                {
                    bool answer = await Shell.Current.DisplayAlert(Language.thong_bao, Language.so_dien_thoai_nay_da_gui_ma_opt_ban_co_muon_tiep_tuc, Language.co, Language.khong);
                    if (!answer) return;

                    foreach (var item in listInvitedUser)
                    {
                        companyService.DeleteInvitedUser(item.Id);// xoa user da gui ma opt
                    }
                    SendInviteCode();
                    return;
                }
                SendInviteCode(); // companyid null
            }
            else
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_nhap_so_dien_thoai, Language.dong);
            }

        }

        public async void SendInviteCode()
        {
            Random random = new Random();
            int ran1 = random.Next(0, 9);
            int ran2 = random.Next(0, 9);
            int ran3 = random.Next(0, 9);
            int ran4 = random.Next(0, 9);
            string opt = $"{ran1}" + $"{ran2}" + $"{ran3}" + $"{ran4}";

            inviteListViewModel.InviteUser.InviteCode = opt;
            inviteListViewModel.InviteUser.PhoneNumber = enNumPhone.Text;
            inviteListViewModel.InviteUser.Status = 0;
            inviteListViewModel.InviteUser.CompanyId = UserLogged.CompanyId;

            ApiResponse apiResponseUserId = await ApiHelper.Get<User>($"{ApiRouter.EMPLOYEE_GET_EMPLOYEE_BY_PHONE}/{enNumPhone.Text}", true);
            User user = new User();

            if (apiResponseUserId.Content != null)
            {
                user = apiResponseUserId.Content as User;
                inviteListViewModel.InviteUser.UserId = user.Id.ToString();
            }
            else
            {
                inviteListViewModel.InviteUser.UserId = null;
            }

            ApiResponse apiResponse = await ApiHelper.Post("api/company/InviteUser", inviteListViewModel.InviteUser, true);
            if (apiResponse.IsSuccess)
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.gui_ma_thanh_cong, Language.dong);
                this.inviteListViewModel.RefreshCommand.Execute(null);
                enNumPhone.Text = null;
                ModalPopup.IsVisible = false;
            }
        }

        public async void MoreAction_Clicked(object sender, EventArgs e)
        {
            User user = (sender as Button).CommandParameter as User;
            var action = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, Language.cap_nhat_nhan_vien, Language.xoa_nhan_vien);
            if (action == Language.cap_nhat_nhan_vien)
            {
                await Shell.Current.Navigation.PushAsync(new AddEmployeePage(user.Id));
            }
            else if (action == Language.xoa_nhan_vien)
            {
                DeleteEmployee(user);
            }
        }
        public async void DeleteEmployee(User user)
        {
            // khong the xoa user hien tai va user admin
            if (UserLogged.Id == user.Id.ToString())
            {
                await DisplayAlert(Language.thong_bao, Language.xoa_khong_thanh_cong, Language.dong);
                return;
            }

            var answer = await DisplayAlert(Language.xac_nhan_xoa, Language.ban_co_chac_chan_muon_xoa_nhan_vien_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            ICompanyService companyService = DependencyService.Get<ICompanyService>();
            List<InviteUser> listInviteUser = companyService.GetListInvitedUser(user.Phone, UserLogged.CompanyId);
            if (listInviteUser.Count != 0)
            {
                foreach (var item in listInviteUser)// xoa inviteuser khi nhan vien nay bi xoa khoi  cty
                {
                    companyService.DeleteInvitedUser(item.Id);
                }
            }

            var deleteResponse = await ApiHelper.Put("api/employee/" + user.Id, false);
            if (deleteResponse.IsSuccess)
            {
                ToastMessageHelper.ShortMessage(Language.xoa_nhan_vien_thanh_cong);
                this.lvDanhSachNhanVien.RefreshCommand.Execute(null);
            }
        }

        public async void CloseModal_Clicked(object sender, EventArgs e)
        {
            ModalPopup.IsVisible = false;
        }
    }
}
