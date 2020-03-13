using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SundihomeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectListPage : ContentPage
    {
        private readonly ProjectListPageViewModel viewModel;
        public ProjectListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ProjectListPageViewModel();
            lv.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as Project;
                await Shell.Current.Navigation.PushAsync(new ProjectDetailPage(item.Id));
            };

            Init();
        }

        public ProjectListPage(bool MyProjectList)
        {
            InitializeComponent();
            Title = Language.du_an_cua_toi;
            this.BindingContext = viewModel = new ProjectListPageViewModel(true);
            lv.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as Project;
                await Shell.Current.Navigation.PushAsync(new AddProjectPage(item.Id));
            };
            lv.Header = null;
            Init();
        }


        public async void Init()
        {
            await viewModel.LoadData();

            MessagingCenter.Subscribe<AddProjectPage, Guid>(this, "OnDeleteSuccess", async (sender, ProjectId) =>
              {
                  loadingPopup.IsVisible = true;
                  if (this.viewModel.Data.Any(x => x.Id == ProjectId))
                  {
                      var deleteProject = this.viewModel.Data.Single(x => x.Id == ProjectId);
                      this.viewModel.Data.Remove(deleteProject);
                  }
                  loadingPopup.IsVisible = false;
              });
            MessagingCenter.Subscribe<AddProjectPage>(this, "OnSaveProject", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });

            List<ProjectTypeModel> listLoaiBatDongSan = new List<ProjectTypeModel>(ProjectTypeData.GetListProjectType());
            List<ProjectTypeModel> newListLoaiBatDongSan = new List<ProjectTypeModel>();
            newListLoaiBatDongSan.Add(new ProjectTypeModel(-1, Language.tat_ca));
            foreach (var item in listLoaiBatDongSan)
            {
                newListLoaiBatDongSan.Add(item);
            }
            BindableLayout.SetItemsSource(stListLoaiDuAn, newListLoaiBatDongSan);

            //set mau cho filter "Tat ca"
            var radBorder = stListLoaiDuAn.Children[0] as RadBorder;
            radBorder.BackgroundColor = (Color)App.Current.Resources["MainDarkColor"];
            (radBorder.Content as Label).TextColor = Color.White;

            loadingPopup.IsVisible = false;
        }

        public async void AddProject_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Shell.Current.Navigation.PushAsync(new AddProjectPage());
        }
        public void Search_Clicked(object sender, EventArgs e)
        {
            this.viewModel.Keyword = searchBar.Text;
            this.viewModel.RefreshCommand.Execute(null);
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                if (!string.IsNullOrWhiteSpace(viewModel.Keyword))
                {
                    this.viewModel.Keyword = null;
                    this.viewModel.RefreshCommand.Execute(null);
                }
            }
        }
        public void LoaiBatDongSan_Tapped(object sender, EventArgs e)
        {
            var radBorder = sender as RadBorder;
            TapGestureRecognizer click = radBorder.GestureRecognizers[0] as TapGestureRecognizer;
            short id = (short)click.CommandParameter;

            //set mau cho filter
            Color MainDarkColor = (Color)App.Current.Resources["MainDarkColor"];
            IDictionary<int, Color> color = new Dictionary<int, Color>()
            {
                {id,MainDarkColor }
            };

            //set mau cho cac filter khong click
            var ortherRadBorder = stListLoaiDuAn.Children.Where(x => x != radBorder);
            foreach (RadBorder item in ortherRadBorder)
            {
                item.BackgroundColor = Color.White;
                (item.Content as Label).TextColor = Color.FromHex("#444444");
            }

            //set mau cho filter dang chon
            //var index = stListLoaiDuAn.Children.IndexOf(radBorder);
            radBorder.BackgroundColor = color[id];
            (radBorder.Content as Label).TextColor = Color.White;

            if (id == -1)
            {
                this.viewModel.TypeProject = null;
                this.viewModel.RefreshCommand.Execute(null);
            }
            else
            {
                this.viewModel.TypeProject = id.ToString();
                this.viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
