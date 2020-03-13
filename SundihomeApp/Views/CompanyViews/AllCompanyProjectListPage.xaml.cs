using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class AllCompanyProjectListPage : ContentPage
    {
        private ListViewPageViewModel2<Project> viewModel;
        public AllCompanyProjectListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewPageViewModel2<Project>()
            {
                PreLoadData = new Command(() => viewModel.ApiUrl = $"api/project/companies?page={viewModel.Page}")
            };
            Init();
        }
        public async void Init()
        {
            LvData.ItemTapped += LvData_ItemTapped;
            MessagingCenter.Subscribe<AddProjectPage, Guid>(this, "OnDeleteSuccess", async (sender, PostId) =>
            {
                var deleteProject = this.viewModel.Data.SingleOrDefault(x => x.Id == PostId);
                if (deleteProject != null)
                    this.viewModel.Data.Remove(deleteProject);
            });
            MessagingCenter.Subscribe<AddProjectPage>(this, "OnSaveProject", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var project = e.Item as Project;
            await Navigation.PushAsync(new ProjectDetailPage(project.Id));
        }

        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged)
            {
                if (UserLogged.RoleId == 0)
                {
                    await Shell.Current.Navigation.PushAsync(new AddProjectPage(true));
                }
                else
                {
                    await Shell.Current.Navigation.PushAsync(new AddProjectPage());
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
        }
    }
}
