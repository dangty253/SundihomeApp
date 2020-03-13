using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews.QuanLyCongTyViews
{
    public partial class PostListContentView : ContentView
    {
        private Guid CompanyId = Guid.Parse(UserLogged.CompanyId);
        public PostListContentView()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            if (UserLogged.RoleId == 0) // owner
            {
                SegmentFilter.ItemsSource = new List<string>() {
                    Language.moi,Language.cam_ket,Language.cho_duyet,Language.gio_chung
                };

                SegmentFilter.SetActive(0);
                SegmentFilter.IsVisible = true;
                MoiView.Content = new FilterCompanyPostListView(false, 1);
                MoiView.IsVisible = true;
                StackButton.IsVisible = true;
            }
            else // nhan vien.
            {
                GioiChungView.Content = new FilterCompanyPostListView(null, 2); // bang 2 la gio chung.
                GioiChungView.IsVisible = true;
            }
            loadingPopup.IsVisible = false;
        }

        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new PostPage(0, Guid.Parse(UserLogged.CompanyId)));
        }

        private void SegmentFilter_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var index = SegmentFilter.GetCurrentIndex();

            MoiView.IsVisible = false;
            CamKetView.IsVisible = false;
            ChoDuyetView.IsVisible = false;
            GioiChungView.IsVisible = false;

            if (index == 0)
            {
                MoiView.IsVisible = true;
            }
            else if (index == 1)
            {
                if (CamKetView.Content == null)
                {
                    CamKetView.Content = new FilterCompanyPostListView(true, 1);
                }
                CamKetView.IsVisible = true;
            }
            else if (index == 2)
            {
                if (ChoDuyetView.Content == null)
                {
                    ChoDuyetView.Content = new FilterCompanyPostListView(null, 0); // 0 la chua duyet
                }
                ChoDuyetView.IsVisible = true;
            }
            else if (index == 3)
            {
                if (GioiChungView.Content == null)
                {
                    GioiChungView.Content = new FilterCompanyPostListView(null, 2); // bang 2 la gio chung.
                }
                GioiChungView.IsVisible = true;
            }
        }
    }
}
