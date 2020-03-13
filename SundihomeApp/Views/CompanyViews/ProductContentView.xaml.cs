using System;
using System.Collections.Generic;
using SundihomeApp.Settings;
using SundihomeApp.Views.CompanyViews.QuanLyCongTyViews;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class ProductContentView : ContentView
    {
        private CompanyProjectListContentView companyProjectListContentView;
        private CompanyPostListContentView CompanyPostListContentView;
        private ProductListContentView productListContentView;
        private Guid _id;
        public ProductContentView(Guid Id)
        {
            InitializeComponent();
            _id = Id;
            companyProjectListContentView = new CompanyProjectListContentView(_id);
            ProjectContentView.Content = companyProjectListContentView;
        }
        private void SegmentSelected_Tapped(object sender, EventArgs e)
        {
            foreach (RadBorder radBorder in Segment.Children)
            {
                radBorder.BackgroundColor = Color.FromHex("#fff");
                radBorder.BorderColor = Color.FromHex("#c3c3c3");
            }

            RadBorder radBorderSelected = sender as RadBorder;
            TapGestureRecognizer tap = radBorderSelected.GestureRecognizers[0] as TapGestureRecognizer;
            int index = int.Parse(tap.CommandParameter.ToString());


            radBorderSelected.BackgroundColor = Color.FromHex("#dadada");
            radBorderSelected.BorderColor = Color.FromHex("#c3c3c3");

            if (index == 0)
            {
                ProjectContentView.IsVisible = true;
                ListBatDongSanContentView.IsVisible = false;
                ProductContentViews.IsVisible = false;
            }
            else if (index == 1)
            {
                if (CompanyPostListContentView == null)
                {
                    CompanyPostListContentView = new CompanyPostListContentView(_id);
                    ListBatDongSanContentView.Content = CompanyPostListContentView;
                }

                ProjectContentView.IsVisible = false;
                ListBatDongSanContentView.IsVisible = true;
                ProductContentViews.IsVisible = false;
            }
            else if (index == 2)
            {
                if (productListContentView == null)
                {
                    productListContentView = new ProductListContentView(_id, false);
                    ProductContentViews.Content = productListContentView;
                }
                ProjectContentView.IsVisible = false;
                ListBatDongSanContentView.IsVisible = false;
                ProductContentViews.IsVisible = true;
            }
        }
    }
}
