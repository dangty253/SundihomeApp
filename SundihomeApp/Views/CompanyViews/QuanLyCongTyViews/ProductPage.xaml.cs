using System;
using System.Collections.Generic;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews.QuanLyCongTyViews
{
    public partial class ProductPage : ContentPage
    {
        private CompanyProjectListContentView companyProjectListContentView;
        private PostListContentView companyPostListContentView;
        private ProductListContentView productListContentView;

        public ProductPage()
        {
            InitializeComponent();
            projectImage.Source = ApiConfig.CloudStorageApiCDN + "/icon/project.png";
            postImage.Source = ApiConfig.CloudStorageApiCDN + "/icon/home.png";
            furnitureImage.Source = ApiConfig.CloudStorageApiCDN + "/icon/furniture.png";
            companyProjectListContentView = new CompanyProjectListContentView(Guid.Parse(UserLogged.CompanyId));
            ProjectContentView.Content = companyProjectListContentView;
            SpanDropdownButotn.Text = Language.du_an;
        }
        private async void OpenTopModal_Tapped(object sender, EventArgs e)
        {
            await topModal.Show();
        }

        private async void SegmentSelected_Tapped(object sender, EventArgs e)
        {
            var radBorder = sender as RadBorder;
            int index = int.Parse(((TapGestureRecognizer)radBorder.GestureRecognizers[0]).CommandParameter.ToString());
            if (index == 0)
            {
                ProjectContentView.IsVisible = true;
                PostContentView.IsVisible = false;
                ProductContentView.IsVisible = false;

                SpanDropdownButotn.Text = Language.du_an;
            }
            else if (index == 1)
            {
                if (companyPostListContentView == null)
                {
                    companyPostListContentView = new PostListContentView();
                    PostContentView.Content = companyPostListContentView;
                }

                ProjectContentView.IsVisible = false;
                PostContentView.IsVisible = true;
                ProductContentView.IsVisible = false;

                SpanDropdownButotn.Text = Language.bat_dong_san;
            }
            else if (index == 2)
            {
                if (productListContentView == null)
                {
                    productListContentView = new ProductListContentView(Guid.Parse(UserLogged.CompanyId), false);
                    ProductContentView.Content = productListContentView;
                }
                ProjectContentView.IsVisible = false;
                PostContentView.IsVisible = false;
                ProductContentView.IsVisible = true;

                SpanDropdownButotn.Text = Language.san_pham;
            }
            await topModal.Hide();
        }
    }
}
