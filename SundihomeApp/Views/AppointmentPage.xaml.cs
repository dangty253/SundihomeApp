using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class AppointmentPage : ContentPage
    {
        public AppointmentPageViewModel viewModel;
        private Guid _id;
        public AppointmentPage(Guid Id)
        {
            _id = Id;
            Init();
            InitUpdate();

        }

        public AppointmentPage(Post post)
        {
            Init();
            InitAdd(post);
        }

        public void Init()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AppointmentPageViewModel();
            calendar.Culture = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
        }
        public async void InitAdd(Post post)
        {
            this.Title = "Lịch xem bất động sản";
            viewModel.Appointment = new Appointment();
            // gia tri ko thay doi
            viewModel.Appointment.OwnerId = post.User.Id;
            viewModel.Appointment.Owner = post.User;
            viewModel.Appointment.PostId = post.Id;
            viewModel.Appointment.Post = post;
            viewModel.Appointment.BuyerId = Guid.Parse(UserLogged.Id);
            viewModel.Appointment.CreatedDate = DateTime.Now;




            PostTitle.Text = post.Title;
            PostAddress.Text = post.Address;

            UserAvatar.Source = post.User.AvatarFullUrl;

            PostUserFullName.Text = post.User.FullName;
            PostUserEmail.Text = post.User.Email;
            PostUserPhone.Text = post.User.Phone;

            EntryEmail.Text = UserLogged.Email;
            EntryPhone.Text = UserLogged.Phone;
            EntryFullName.Text = UserLogged.FullName;




            loadingPopup.IsVisible = false;
        }

        public async void InitUpdate()
        {
            this.Title = "Lịch xem bất động sản";
            LabelSave.Text = "Cập nhật lịch";

            viewModel.Appointment = await viewModel.LoadAppointmentAsync(_id);
            if (viewModel.Appointment == null)
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", "Không tìm thấy cuộc hẹn", Language.dong);
                return;
            }
            PostTitle.Text = viewModel.Appointment.Post.Title;
            PostAddress.Text = viewModel.Appointment.Post.Address;

            UserAvatar.Source = AvatarHelper.GetUserAvatar(viewModel.Appointment.Owner.AvatarUrl);

            PostUserFullName.Text = viewModel.Appointment.Owner.FullName;
            PostUserEmail.Text = viewModel.Appointment.Owner.Email;
            PostUserPhone.Text = viewModel.Appointment.Owner.Phone;

            EntryEmail.Text = viewModel.Appointment.BuyerEmail;
            EntryPhone.Text = viewModel.Appointment.BuyerPhone;
            EntryFullName.Text = viewModel.Appointment.BuyerFullName;
            EdtDescription.Text = viewModel.Appointment.Description;

            calendar.SelectedDate = viewModel.Appointment.Date;


            if (viewModel.Appointment.BuyerId == Guid.Parse(UserLogged.Id)) // buyier = nguoi tao lich.
            {

            }
            else
            {
                EntryFullName.IsVisible = EntryEmail.IsVisible = EntryPhone.IsVisible = BorderDescription.IsVisible = false;
                LblFullName.IsVisible = LblEmail.IsVisible = LblPhone.IsVisible = LblDescription.IsVisible = true;

                LblFullName.Text = viewModel.Appointment.BuyerEmail;
                LblEmail.Text = viewModel.Appointment.BuyerPhone;
                LblPhone.Text = viewModel.Appointment.BuyerFullName;
                LblDescription.Text = viewModel.Appointment.Description;
            }
            loadingPopup.IsVisible = false;
        }


        private async void Save_Appointment_Clicked(object sender, EventArgs e)
        {
            if (EntryFullName.IsVisible && string.IsNullOrWhiteSpace(EntryFullName.Text))
            {
                await DisplayAlert("", "Nhập họ tên", Language.dong);
                return;
            }
            if (EntryPhone.IsVisible && string.IsNullOrWhiteSpace(EntryPhone.Text))
            {
                await DisplayAlert("", "Nhập số điện thoại", Language.dong);
                return;
            }

            loadingPopup.IsVisible = true;

            viewModel.Appointment.BuyerFullName = EntryFullName.Text;
            viewModel.Appointment.BuyerEmail = EntryEmail.Text;
            viewModel.Appointment.BuyerPhone = EntryPhone.Text;
            viewModel.Appointment.Description = EdtDescription.Text;
            viewModel.Appointment.Date = calendar.SelectedDate;

            ApiResponse response = null;
            Guid SaveId = Guid.Empty;
            Guid ReceiverId = Guid.Empty;
            string ReceiverToken = string.Empty;
            if (viewModel.Appointment.OwnerId == Guid.Parse(UserLogged.Id)) // toi la nguoi ban - toi them/sua
            {
                ReceiverId = viewModel.Appointment.BuyerId;
                ReceiverToken = viewModel.Appointment.Buyer.FirebaseRegToken;
            }
            else
            {
                ReceiverId = viewModel.Appointment.OwnerId;
                ReceiverToken = viewModel.Appointment.Owner.FirebaseRegToken;
            }

            if (viewModel.Appointment.Id == Guid.Empty)
            {
                response = await ApiHelper.Post("api/appointment", viewModel.Appointment, true);
            }
            else
            {
                response = await ApiHelper.Put("api/appointment", viewModel.Appointment, true);
            }

            if (response.IsSuccess)
            {
                NotificationService notificationService = new NotificationService();

                string message = string.Empty;
                if (viewModel.Appointment.Id == Guid.Empty)
                {
                    SaveId = Guid.Parse(response.Content.ToString());
                    message = UserLogged.FullName + " đã đăt một cuộc hẹn với bạn";
                }
                else
                {
                    message = UserLogged.FullName + " đã cập nhật lịch hẹn";
                    SaveId = _id;
                }

                var notification = new NotificationModel()
                {
                    UserId = ReceiverId,
                    CurrentBadgeCount = (int)notificationService.CountNotReadNotificationUser(ReceiverId) + 1,
                    Title = message,
                    NotificationType = NotificationType.ViewAppointment,
                    AppointmentId = SaveId,
                    CreatedDate = DateTime.Now,
                    IsRead = false,
                    Thumbnail = viewModel.Appointment.Post.AvatarFullUrl
                };

                await notificationService.AddNotification(notification, Language.lich);

                loadingPopup.IsVisible = false;
                if (viewModel.Appointment.Id == Guid.Empty)
                {
                    await DisplayAlert("", "Đặt lịch thành công", Language.dong);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("", "Cập nhật lịch thành công", Language.dong);
                }
            }
            else
            {
                loadingPopup.IsVisible = false;
                await DisplayAlert("", response.Message, Language.dong);
            }
        }
    }
}
