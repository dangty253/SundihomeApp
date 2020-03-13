using System;
using System.Collections.Generic;
using System.Linq;
using Stormlion.PhotoBrowser;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class CompanyImageListContentView : ContentView
    {
        public List<Photo> Photos { get; set; }
        public PhotoBrowser PhotoBrowser = null;
        public string[] _imageList { get; set; }
        public CompanyImageListContentView(string[] ImageList)
        {
            InitializeComponent();
            ImageListView.FlowItemsSource = ImageList;
            ImageListView.FlowItemTapped += ImageListView_FlowItemTapped;


            _imageList = ImageList;
            this.Photos = ImageList.Select(x => new Photo()
            {
                Title = "",
                URL = x
            }).ToList();
        }

        private void ImageListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            if (PhotoBrowser == null)
            {
                PhotoBrowser = new PhotoBrowser
                {
                    Photos = Photos,
                    EnableGrid = true,
                };
            }
            var item = e.Item as string;
            int index = _imageList.ToList().IndexOf(item);
            PhotoBrowser.StartIndex = index;
            PhotoBrowser.Show();
        }
    }
}
