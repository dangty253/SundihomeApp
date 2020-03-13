using System;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Views;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class AppointmentListPageViewModel : ListViewPageViewModel2<Appointment>
    {
        public AppointmentListPageViewModel()
        {
            this.PreLoadData = new Command(() => { ApiUrl = $"api/appointment?page={Page}"; });
        }
    }
}
