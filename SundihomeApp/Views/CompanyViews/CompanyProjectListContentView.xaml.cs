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
    public partial class CompanyProjectListContentView : ContentView
    {
        public ListViewPageViewModel2<Project> viewModel;
        private Guid _companyId;
        private bool _isOwner = false;
        public string Keyword;
        public CompanyProjectListContentView(Guid CompanyId)
        {
            InitializeComponent();
            _companyId = CompanyId;
            this.BindingContext = viewModel = new ListViewPageViewModel2<Project>()
            {
                PreLoadData = new Command(() =>
                {
                    string Url = ApiRouter.PROJECT_GET_BYCOMPANYID + "/" + CompanyId + "?page=" + viewModel.Page;
                    if (!string.IsNullOrWhiteSpace(Keyword))
                    {
                        Url += $"&keyword={Keyword}";
                    }
                    viewModel.ApiUrl = Url;
                })
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
            if (UserLogged.RoleId == 0 && Guid.Parse(UserLogged.CompanyId) == this._companyId)
            {
                _isOwner = true;
                StackButton.IsVisible = true;
            }
            loadingPopup.IsVisible = false;
        }

        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var project = e.Item as Project;

            if (_isOwner)
            {
                string[] options = { Language.xem_chi_tiet, Language.chinh_sua };
                string action = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, options);

                if (action == options[0])
                {
                    await Navigation.PushAsync(new ProjectDetailPage(project.Id));
                }
                else if (action == options[1])
                {
                    await Shell.Current.Navigation.PushAsync(new AddProjectPage(project.Id));
                }
            }
            else
            {
                await Navigation.PushAsync(new ProjectDetailPage(project.Id));
            }
        }

        private async void AddProject_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AddProjectPage(true));
        }
        private void Search_Pressed(object sender, EventArgs e)
        {
            Keyword = searchBar.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text) && !string.IsNullOrWhiteSpace(Keyword))
            {
                Keyword = null;
                viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
