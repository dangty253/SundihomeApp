using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class AddTaskPageViewModel: BaseViewModel
    {
        public ObservableCollection<Contact> ContactList { get; set; }

        private CongViec _congViec;
        public CongViec CongViec
        {
            get => _congViec;
            set
            {
                _congViec = value;
                OnPropertyChanged(nameof(CongViec));
            }
        }

        private TimeSpan _selectedTime;
        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                OnPropertyChanged(nameof(SelectedTime));
            }
        }

        public AddTaskPageViewModel()
        {
            ContactList = new ObservableCollection<Contact>();
            CongViec = new CongViec() { Date = DateTime.Now };
            
        }

        public async Task GetContactList()
        {
            ApiResponse response = await ApiHelper.Get<List<Contact>>(ApiRouter.CONTACT_GETMYCONTACTS, true);
            if (response.IsSuccess)
            {
                List<Contact> list = response.Content as List<Contact>;
                foreach (var item in list)
                {
                    ContactList.Add(item);
                }
            }
        }

        public async Task GetCongViec(Guid taskId)
        {
            ApiResponse response = await ApiHelper.Get<CongViec>($"{ApiRouter.TASK_CRUD}/{taskId}", true);
            if (response.IsSuccess)
            {
                DateTime date = ((CongViec)response.Content).Date;
                CongViec = response.Content as CongViec;
                CongViec.Date = date;
                SetSelectedTime();
            }
            else
            {
                await Shell.Current.DisplayAlert("", response.Message, Language.dong);
            }
        }

        public void SetStatus(DateTime time)
        {
            CongViec.Status = 0;
            DateTime now = DateTime.Now;
            if(DateTime.Compare(time, now) < 0)
            {
                CongViec.Status = 1;
            }
        }

        public void SetSelectedTime()
        {
            SelectedTime = new TimeSpan(CongViec.Date.Hour, CongViec.Date.Minute, CongViec.Date.Second);
        }

        public void SetDateTime()
        {
            CongViec.Date = new DateTime(CongViec.Date.Year, CongViec.Date.Month, CongViec.Date.Day, SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds);
        }
    }
}
