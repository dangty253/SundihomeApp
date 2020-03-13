using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public ObservableCollection<CongViec> Tasks { get; set; }  // ban
        public ObservableCollection<ContactNeed> ContactNeeds { get; set; }
        public Company Company { get; set; }
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    await Task.WhenAll(this.LoadTasks(), LoadContactNeeds());
                    IsRefreshing = false;
                });
            }
        }
        public HomePageViewModel()
        {
            Tasks = new ObservableCollection<CongViec>();
            ContactNeeds = new ObservableCollection<ContactNeed>();
        }

        public async Task LoadTasks()
        {
            Tasks.Clear();
            var response = await ApiHelper.Get<List<CongViec>>(ApiRouter.TASK_NEWTASKS, true);
            if (response.IsSuccess)
            {
                var data = response.Content as List<CongViec>;
                for (int i = 0; i < data.Count; i++)
                {
                    Tasks.Add(data[i]);
                }
            }
        }

        public async Task LoadContactNeeds()
        {
            ContactNeeds.Clear();
            var response = await ApiHelper.Get<List<ContactNeed>>(ApiRouter.EMPLOYEE_GETNEWCONTACTNEEDS, true);
            if (response.IsSuccess)
            {
                var data = response.Content as List<ContactNeed>;
                for (int i = 0; i < data.Count; i++)
                {
                    ContactNeeds.Add(data[i]);
                }
            }
        }
    }
}
