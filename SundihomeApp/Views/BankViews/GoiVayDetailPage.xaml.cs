using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views.BankViews
{
    public partial class GoiVayDetailPage : ContentPage
    {
        private Guid _id;
        private GoiVay _goiVay;
        public GoiVayDetailPage(Guid Id)
        {
            InitializeComponent();
            _id = Id;
            Init();
        }


        public async Task SetForm()
        {
            var response = await ApiHelper.Get<GoiVay>(ApiRouter.BANK_GOIVAY + "/" + _id);
            if (response.IsSuccess && response.Content != null)
            {
                this._goiVay = response.Content as GoiVay;
                this.Title = _goiVay.Name;
                if (!_goiVay.Image.StartsWith("bank_logo"))
                {
                    image.Aspect = Aspect.AspectFill;
                }

                image.Source = _goiVay.ImageFullUrl;


                lblNganHang.Text = _goiVay.Bank.Name;
                lblGoiVayName.Text = _goiVay.Name;
                lblMaxPrice.Text = DecimalHelper.DecimalToText(_goiVay.MaxPrice) + "%";
                lblMaxTime.Text = _goiVay.MaxTime + " " + (_goiVay.MaxTimeUnit == 0 ? Language.year.ToLower() : Language.month.ToLower());
                lblLaiSuat.Text = DecimalHelper.DecimalToText(_goiVay.LaiSuat) + "%";
                lblCondition.Text = _goiVay.Condition;
                lblDescription.Text = _goiVay.Description;

                lblEmpName.Text = _goiVay.User.FullName;
                lblPhone.Text = _goiVay.User.Phone;


                imageAvatar.Source = _goiVay.User.AvatarFullUrl;

                lblAddress.Text = _goiVay.Employee.Address;
            }
        }
        public async void Init()
        {
            await SetForm();
            if (_goiVay == null) return;


            var ButtonCommandList = new List<FloatButtonItem>();
            if (UserLogged.IsLogged && _goiVay.EmployeeId == Guid.Parse(UserLogged.Id))
            {
                ButtonCommandList.Add(new FloatButtonItem(Language.chinh_sua, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, async (sender, e) =>
                {
                    var view = new AddLoanView(bottomModal, _id);
                    view.OnCancel += async (sender1, e1) => await ModalAddLoan.Hide();
                    view.OnSaved += async (sender1, e1) =>
                    {
                        loadingPopup.IsVisible = true;
                        await ModalAddLoan.Hide();
                        await SetForm();
                        loadingPopup.IsVisible = false;
                    };
                    ModalAddLoan.Body = view;
                    await ModalAddLoan.Show();
                }));
                ButtonCommandList.Add(new FloatButtonItem(Language.xoa, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2ed", null, Remove_Clicked));
            }
            else
            {
                ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, CallOptionSelected));//2
                ButtonCommandList.Add(new FloatButtonItem(Language.chat, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, ChatOptionSelected));//3
                ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessageOptionSelected));//4
            }
            floatingButtonGroup.ItemsSource = ButtonCommandList;
            loadingPopup.IsVisible = false;
        }

        private async void Remove_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.xac_nhan_xoa, Language.ban_co_chac_chan_muon_xoa_khong, Language.xoa, Language.huy);
            if (!answer) return;

            var response = await ApiHelper.Delete(ApiRouter.BANK_GOIVAY + "/" + this._id);
            if (response.IsSuccess)
            {
                MessagingCenter.Send<GoiVayDetailPage, Guid>(this, "OnDeleted", this._id);
                await Navigation.PopAsync();
            }
        }

        public async void CallOptionSelected(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            try
            {
                PhoneDialer.Open(_goiVay.User.Phone);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        //event của button nhắn tin
        public async void ChatOptionSelected(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new ChatPage(_goiVay.User.Id.ToString()));
        }

        // cho gui tin nhan
        public async void SendMessageOptionSelected(object sender, EventArgs e)
        {

            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            try
            {
                await Sms.ComposeAsync(new SmsMessage(string.Empty, _goiVay.User.Phone));
            }
            catch
            {
                await DisplayAlert("Lỗi", Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }
    }
}
