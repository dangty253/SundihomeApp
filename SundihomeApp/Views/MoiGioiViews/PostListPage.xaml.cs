using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class PostListPage : ContentPage
    {
        public string Keyword;
        public PostListPage()
        {
            InitializeComponent();
            moiView.Content = new FilterEmployeePostListView(false, false);

            List<string> listOption = new List<string>()
            {
                Language.moi,Language.cam_ket
            };

            if (string.IsNullOrEmpty(UserLogged.CompanyId) == false) // moi gioi chua dang ky cong ty.
            {
                listOption.Add(Language.moi + "(CT)");
                listOption.Add(Language.cam_ket + "(CT)");
            }
            Segment.ItemsSource = listOption;
            Segment.SetActive(0);
        }

        private async void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            await Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        private void Segment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int CurrentIndex = Segment.GetCurrentIndex();

            moiView.IsVisible = camKetView.IsVisible = moiCompanyView.IsVisible = camKetComapnyView.IsVisible = false;

            if (CurrentIndex == 0)
            {
                moiView.IsVisible = true;
            }
            else if (CurrentIndex == 1)
            {
                if (camKetView.Content == null)
                {
                    camKetView.Content = new FilterEmployeePostListView(true, false);
                }
                camKetView.IsVisible = true;
            }
            else if (CurrentIndex == 2)
            {
                if (moiCompanyView.Content == null)
                {
                    moiCompanyView.Content = new FilterEmployeePostListView(false, true);
                }
                moiCompanyView.IsVisible = true;
            }
            else if (CurrentIndex == 3)
            {
                if (camKetComapnyView.Content == null)
                {
                    camKetComapnyView.Content = new FilterEmployeePostListView(true, true);
                }
                camKetComapnyView.IsVisible = true;
            }
        }

        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new PostPage());
        }
    }
}
