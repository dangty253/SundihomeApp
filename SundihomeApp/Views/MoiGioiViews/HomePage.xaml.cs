using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApp.Helpers;
using SundihomeApp.Settings;
using SundihomeApp.Views.QuanLyMoiGioiViews;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class HomePage : ContentPage
    {
        public ViewModels.MoiGioiViewModels.HomePageViewModel viewModel;
        public HomePage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ViewModels.MoiGioiViewModels.HomePageViewModel();
            Init();
        }
        public async void Init()
        {
            await Task.WhenAll(viewModel.LoadTasks(), viewModel.LoadContactNeeds());
            MessagingCenter.Subscribe<ViewModels.MoiGioiViewModels.AddContactNeedContentViewModel>(this, "ReloadNhuCauList", async (sender) =>
            {
                await this.viewModel.LoadContactNeeds();
            });
            MessagingCenter.Subscribe<AddTaskPage, SundihomeApi.Entities.MoiGioiEntities.CongViec>(this, "AddTask", async (sender, cv) =>
            {
                await this.viewModel.LoadTasks();
            });
            MessagingCenter.Subscribe<AddTaskPage, SundihomeApi.Entities.MoiGioiEntities.CongViec>(this, "UpdateTask", async (sender, cv) =>
            {
                await this.viewModel.LoadTasks();
            });
            MessagingCenter.Subscribe<TaskDetailPage, Guid>(this, "DeleteTask", async (sender, id) => await this.viewModel.LoadTasks());
            MessagingCenter.Subscribe<ContactListPage, Guid>(this, "DeleteContact", async (sender, id) =>
            {
                await this.viewModel.LoadTasks();
                await this.viewModel.LoadContactNeeds();
            });
            MessagingCenter.Subscribe<TaskDetailPage, Guid>(this, "CompletedTask", async (sender, id) =>
            {
                await this.viewModel.LoadTasks();
            });
            loadingPopup.IsVisible = false;
        }


        public async void ViewMoreType01_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//quanlymoigioi/congviec");
        }
        public async void GoTo_PostDetail_Cliked(object sender, EventArgs e)
        {
            var stacklayout = sender as StackLayout;
            var tap = stacklayout.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)tap.CommandParameter;
            await Shell.Current.Navigation.PushAsync(new PostDetailPage(id));
        }

        private async void ViewAllContactNeeds_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//quanlymoigioi/khachhang");
        }
        private async void ViewContact_Click(object sender, EventArgs e)
        {
            var contactId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await Navigation.PushAsync(new ContactDetailPage(contactId, false));
        }

        private async void ViewProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ThongTinMoiGioiPage(Guid.Parse(UserLogged.Id), true));
        }
        private async void ViewTask_Clicked(object sender, EventArgs e)
        {
            var taskId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await Navigation.PushAsync(new TaskDetailPage(taskId));
        }
    }
}
