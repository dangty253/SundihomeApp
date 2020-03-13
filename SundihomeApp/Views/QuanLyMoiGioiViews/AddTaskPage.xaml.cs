using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using Telerik.XamarinForms.Primitives.CheckBox;
using Xamarin.Forms;

namespace SundihomeApp.Views.QuanLyMoiGioiViews
{
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPageViewModel viewModel;
        public AddTaskPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new AddTaskPageViewModel();
            InitAdd();
        }

        public AddTaskPage(Guid taskId)
        {
            InitializeComponent();
            BindingContext = viewModel = new AddTaskPageViewModel();
            InitUpdate(taskId);
        }

        private async void InitAdd()
        {
            await viewModel.GetContactList();
            loadingPopup.IsVisible = false;
        }

        private async void InitUpdate(Guid taskId)
        {
            await Task.WhenAll(viewModel.GetContactList(),
                viewModel.GetCongViec(taskId));
            Title = viewModel.CongViec.Title;
            loadingPopup.IsVisible = false;
        }

        public void Contact_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.CongViec.Contact != null)
            {
                viewModel.CongViec.ContactId = viewModel.CongViec.Contact.Id;
            }
            else
            {
                viewModel.CongViec.Contact = null;
                viewModel.CongViec.ContactId = Guid.Empty;
            }
        }

        private async void SaveTask_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;

            viewModel.SetDateTime();
            viewModel.SetStatus(viewModel.CongViec.Date);
            viewModel.CongViec.MoiGioiId = Guid.Parse(UserLogged.Id);

            CongViec model = new CongViec();
            model.Id = viewModel.CongViec.Id;
            model.Title = viewModel.CongViec.Title;
            model.Description = viewModel.CongViec.Description;
            model.ContactId = viewModel.CongViec.ContactId;
            model.Status = viewModel.CongViec.Status;
            model.Date = viewModel.CongViec.Date;
            model.MoiGioiId = viewModel.CongViec.MoiGioiId;

            //viewModel.CongViec.Contact = null;

            if (string.IsNullOrWhiteSpace(viewModel.CongViec.Title))
            {
                await DisplayAlert("", Language.vui_long_nhap_tieu_de, Language.dong);
                loadingPopup.IsVisible = false;
                return;
            }
            if (viewModel.CongViec.ContactId == Guid.Empty)
            {
                await DisplayAlert("", Language.vui_long_chon_khach_hang, Language.dong);
                loadingPopup.IsVisible = false;
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.CongViec.Description))
            {
                await DisplayAlert("", Language.vui_long_nhap_noi_dung, Language.dong);
                loadingPopup.IsVisible = false;
                return;
            }
            if (viewModel.CongViec.Status == 1)
            {
                await DisplayAlert("", Language.thoi_gian_khong_hop_le, Language.dong);
                loadingPopup.IsVisible = false;
                return;
            }

            if (viewModel.CongViec.Id != Guid.Empty)
            {
                //Update
                ApiResponse response = await ApiHelper.Put(ApiRouter.TASK_CRUD, model, true);
                if (response.IsSuccess)
                {
                    MessagingCenter.Send<AddTaskPage, CongViec>(this, "UpdateTask", viewModel.CongViec);
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_cong_viec_thanh_cong);
                }
                else
                {
                    await DisplayAlert("", Language.cap_nhat_cong_viec_khong_thanh_cong, Language.dong);
                }
                loadingPopup.IsVisible = false;
            }
            else
            {
                //Add
                model.Id = viewModel.CongViec.Id = Guid.NewGuid();
                ApiResponse response = await ApiHelper.Post(ApiRouter.TASK_CRUD, model, true);
                if (response.IsSuccess)
                {
                    MessagingCenter.Send<AddTaskPage, CongViec>(this, "AddTask", viewModel.CongViec);
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage(Language.them_cong_viec_thanh_cong);
                }
                else
                {
                    await DisplayAlert("", Language.them_cong_viec_that_bai, Language.dong);
                }
                loadingPopup.IsVisible = false;

            }
        }
    }
}
