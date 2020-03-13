using System;
using System.Collections.Generic;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.QuanLyMoiGioiViews
{
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPageViewModel viewModel;
        private Guid _taskId;

        public TaskDetailPage(Guid taskId)
        {
            InitializeComponent();
            BindingContext = viewModel = new TaskDetailPageViewModel();
            _taskId = taskId;
            Init();
        }

        public async void Init()
        {
            await viewModel.GetCongViec(_taskId);
            if (viewModel.CongViec.Status == 0)
            {
                btnEdit.IsVisible = true;
                btnCompleted.IsVisible = true;
            }
            loadingPopup.IsVisible = false;

            MessagingCenter.Subscribe<AddTaskPage, CongViec>(this, "UpdateTask", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                viewModel.CongViec = arg;
                if (viewModel.CongViec.Status != 0)
                {
                    btnEdit.IsVisible = false;
                }
                loadingPopup.IsVisible = false;
            });
        }

        private async void EditTask_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage(_taskId));
        }

        private async void DeleteTask_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            var answer = await DisplayAlert("", Language.ban_co_chac_muon_xoa_cong_viec_nay_khong, Language.dong_y, Language.huy);
            if (answer)
            {
                ApiResponse response = await ApiHelper.Delete($"{ApiRouter.TASK_CRUD}/{_taskId}", true);
                if (response.IsSuccess)
                {
                    MessagingCenter.Send<TaskDetailPage, Guid>(this, "DeleteTask", _taskId);
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage(Language.xoa_thanh_cong);
                }
                else
                {
                    await DisplayAlert("", response.Message, Language.dong);
                }
            }
            loadingPopup.IsVisible = false;
        }

        private async void CompletedTask_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            ApiResponse response = await ApiHelper.Put($"{ApiRouter.TASK_UPDATE_COMPLETED}/{viewModel.CongViec.Id}", null, true);
            if (response.IsSuccess)
            {
                await viewModel.GetCongViec(_taskId);
                btnCompleted.IsVisible = false;
                btnEdit.IsVisible = false;
                MessagingCenter.Send<TaskDetailPage, Guid>(this, "CompletedTask", _taskId);
                ToastMessageHelper.ShortMessage(Language.da_hoan_thanh);
                loadingPopup.IsVisible = false;
            }
        }
    }
}
