using System;
using System.Collections.Generic;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class OwnerPostPage : ContentPage
    {
        public OwnerPostPageViewModel viewModel;
        public Guid _id;
        public OwnerPostPage(Guid Id)
        {
            InitializeComponent();
            _id = Id;
            Init();
            loadingPopup.IsVisible = false;
        }
        public async void Init()
        {
            this.BindingContext = viewModel = new OwnerPostPageViewModel();
            await viewModel.GetOwnerAsync(_id);
            viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, CallOptionSelected));
            viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessageOptionSelected));
        }
        public void CallOptionSelected(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(viewModel.Owner.OwnerPhone))
                {
                    DisplayAlert("", Language.khong_co_sdt_cua_chu_so_huu_vui_long_thu_lai, Language.dong);
                    return;
                }
                PhoneDialer.Open(viewModel.Owner.OwnerPhone);

            }
            catch (Exception ex) {
                DisplayAlert("",Language.loi_he_thong_vui_long_thu_lai,Language.dong);
            }
            
        }
        public void SendMessageOptionSelected(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(viewModel.Owner.OwnerPhone))
                {
                    DisplayAlert("", Language.khong_co_sdt_cua_chu_so_huu_vui_long_thu_lai, Language.dong);
                    return;
                }
                Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.Owner.OwnerPhone));
            }
            catch {
                DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
            }

        }
    }
}
