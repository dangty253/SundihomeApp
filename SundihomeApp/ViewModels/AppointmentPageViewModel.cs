using System;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Helpers;

namespace SundihomeApp.ViewModels
{
    public class AppointmentPageViewModel : BaseViewModel
    {
        private Appointment _appointment;
        public Appointment Appointment
        {
            get => _appointment;
            set
            {
                _appointment = value;
                OnPropertyChanged(nameof(Appointment));
            }
        }

        public async Task<Appointment> LoadAppointmentAsync(Guid Id)
        {
            var response = await ApiHelper.Get<Appointment>("api/appointment/" + Id, true);
            if (response.IsSuccess)
            {
                return response.Content as Appointment;
            }
            else
            {
                return null;
            }
        }
    }
}
