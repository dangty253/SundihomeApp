using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class AppointmentListPage : ContentPage
    {
        public AppointmentListPageViewModel viewModel;
        public AppointmentListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AppointmentListPageViewModel();
            Init();
        }

        public async void Init()
        {
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        public async void ShowPopup_Clicked(object sender, EventArgs e)
        {
            Appointment appointment = (sender as Button).CommandParameter as Appointment;
            //string[] options = {
            //    "Dẫn đường",
            //    Language.chinh_sua,
            //    "Huỷ hẹn" };
            //string action = await DisplayActionSheet("", Language.dong, null, options);
            //if (action == options[1])
            //{
            //    await Navigation.PushAsync(new AppointmentPage(appointment.Id));
            //}


            await Navigation.PushAsync(new AppointmentPage(appointment.Id));
        }
    }
}
