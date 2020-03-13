using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class ContactDetailPage : ContentPage
    {
        private AddContactNeedContent AddContactNeedContent;
        private ContactDetailContent ContactDetailContent;
        private ContactPostCareContent ContactPostCareContent;
        public int CurrentIndex = 0;
        private Contact _contact;
        private Guid _contactId;
        private bool _isCompany;
        public ContactDetailPage(Guid contactId, Boolean isCompany)
        {
            InitializeComponent();
            _isCompany = isCompany;
            _contactId = contactId;
            InitUpdate();
            Init();


        }
        public async void InitUpdate()
        {
            var apiResponse = await ApiHelper.Get<Contact>($"api/contact/{this._contactId}", true);
            if (apiResponse.IsSuccess == false) return;

            this._contact = apiResponse.Content as Contact;

            if (this._contact == null)
            {
                await DisplayAlert("", Language.khong_tim_thay_khach_hang, Language.dong);
                await Navigation.PopAsync();
            }
            else this.Title = this._contact.FullName;
            if (ContactDetailContent == null)
            {
                ContactDetailContent = new ContactDetailContent(this._contactId);
            }
            MainContentView.Content = ContactDetailContent;

        }
        public  void Init()
        {
            segment.ItemsSource = new List<string> { Language.thong_tin, Language.nhu_cau, Language.bsd_quan_tam };
            segment.SetActive(0);
        }
        public async void SegmentSelected_Tapped(Object Sender, EventArgs e)
        {
            CurrentIndex = segment.GetCurrentIndex();

            MainContentView.IsVisible = MainContentView2.IsVisible = MainContentView3.IsVisible = false;

            switch (CurrentIndex)
            {
                case 0:
                    if (ContactDetailContent == null)
                    {
                        ContactDetailContent = new ContactDetailContent(this._contactId);
                    }
                    MainContentView.IsVisible = true;
                    break;
                case 1:
                    if (MainContentView2.Content == null)
                    {
                        MainContentView2.Content = new AddContactNeedContent(this._contact);
                    }

                    //MainContentView.Content = AddContactNeedContent;
                    MainContentView2.IsVisible = true;
                    break;
                case 2:
                    if (MainContentView3.Content == null)
                    {
                        MainContentView3.Content = new ContactPostCareContent(this._contactId, this._isCompany);
                    }

                    //MainContentView.Content = ContactPostCareContent;
                    MainContentView3.IsVisible = true;
                    break;
            }
        }

    }
}
