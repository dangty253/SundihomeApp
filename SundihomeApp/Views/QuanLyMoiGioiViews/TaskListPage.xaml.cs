using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels.MoiGioiViewModels;
using SundihomeApp.Views.MoiGioiViews;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.QuanLyMoiGioiViews
{
    public partial class TaskListPage : ContentPage
    {
        private int CurrentIndex = 0;
        public TaskListPageViewModel viewModel;

        public TaskListPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new TaskListPageViewModel(0);
            lvCongViec.ItemTapped += TaskList_Tapped;
            Init();

            MessagingCenter.Subscribe<TaskDetailPage, Guid>(this, "CompletedTask", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                if (this.viewModel.Data.Any(x => x.Id == arg))
                {
                    var task = this.viewModel.Data.Single(x => x.Id == arg);
                    this.viewModel.Data.Remove(task);
                }
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<AddTaskPage, CongViec>(this, "UpdateTask", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                if (this.viewModel.Data.Any(x => x.Id == arg.Id))
                {
                    var task = this.viewModel.Data.Single(x => x.Id == arg.Id);
                    this.viewModel.Data.Remove(task);
                }
                if (arg.Status == 0 && viewModel.Type == 0)
                {
                    viewModel.Data.Insert(0, arg);
                }
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<AddTaskPage, CongViec>(this, "AddTask", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModel.Type == 0)
                {
                    viewModel.Data.Insert(0, arg);
                }
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<TaskDetailPage, Guid>(this, "DeleteTask", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                if (this.viewModel.Data.Any(x => x.Id == arg))
                {
                    var task = this.viewModel.Data.Single(x => x.Id == arg);
                    this.viewModel.Data.Remove(task);
                }
                loadingPopup.IsVisible = false;
            });


            MessagingCenter.Subscribe<ContactListPage, Guid>(this, "DeleteContact", async (sender, id) =>
            {
                var tasks = viewModel.Data.Where(x => x.ContactId == id).ToList();
                if (tasks.Any())
                {
                    foreach (var item in tasks)
                    {
                        viewModel.Data.Remove(item);
                    }
                }
            });
        }

        private async void Init()
        {
            await viewModel.LoadData();
            segment.ItemsSource = new List<string> { Language.moi, Language.da_dong };
            segment.SetActive(0);
            loadingPopup.IsVisible = false;
        }

        private async void SegmentSelected_Tapped(object sender, EventArgs e)
        {
            CurrentIndex = segment.GetCurrentIndex();

            if (CurrentIndex == 0)
            {
                //Cong viec moi
                viewModel.Type = 0;
                await viewModel.LoadOnRefreshCommandAsync();
            }
            else
            {
                //Cong viec da dong
                viewModel.Type = 1;
                await viewModel.LoadOnRefreshCommandAsync();
            }
        }

        public async void TaskList_Tapped(object sender, ItemTappedEventArgs e)
        {
            CongViec item = e.Item as CongViec;
            await Navigation.PushAsync(new TaskDetailPage(item.Id));
        }

        private async void AddWork_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage());
        }

    }
}
