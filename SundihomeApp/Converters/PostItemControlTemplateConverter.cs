using System;
using System.Globalization;
using System.Linq;
using FFImageLoading.Forms;
using Stormlion.PhotoBrowser;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class PostItemControlTemplateConverter : IValueConverter
    {

        public string Folder { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string[] Images = (string[])value;

            var grid = new Grid()
            {
                ColumnSpacing = 2,
                RowSpacing = 2,
            };

            var Photos = Images.Select(x => new Photo() { Title = "", URL = Configuration.ApiConfig.CloudStorageApiCDN + "/" + Folder + "/" + x }).ToList();
            if (Images != null)
            {
                int end = 1;
                if (Images.Length == 1)
                {
                    end = 1;
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = 200 });
                }
                else if (Images.Length == 2)
                {
                    end = 2;
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = 150 });
                }
                else if (Images.Length >= 3)
                {
                    end = 3;
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    grid.RowDefinitions.Add(new RowDefinition() { Height = 100 });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = 100 });
                }


                for (int i = 0; i < end; i++)
                {
                    string imageSource = Configuration.ApiConfig.CloudStorageApiCDN + "/" + Folder + "/" + Images[i];
                    var img = new CachedImage()
                    {
                        Aspect = Aspect.AspectFill,
                        HeightRequest = 100,
                        Source = imageSource,
                        DownsampleHeight = 100,
                        DownsampleToViewSize = true
                    };
                    var tap = new TapGestureRecognizer()
                    {
                        NumberOfTapsRequired = 1,
                        CommandParameter = i
                    };
                    tap.Tapped += (o, e) =>
                    {
                        int index = (int)((o as CachedImage).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
                        new PhotoBrowser
                        {
                            Photos = Photos,
                            EnableGrid = true,
                            StartIndex = index,
                        }.Show();
                    };

                    img.GestureRecognizers.Add(tap);

                    Grid.SetRow(img, 0);
                    Grid.SetColumn(img, i);

                    grid.Children.Add(img);
                }

                if (grid.RowDefinitions.Count == 2)
                {
                    Grid.SetRowSpan(grid.Children[0], 2);
                    Grid.SetColumn(grid.Children[1], 1);
                    Grid.SetRow(grid.Children[1], 0);

                    Grid.SetColumn(grid.Children[2], 1);
                    Grid.SetRow(grid.Children[2], 1);
                }

            }

            return grid;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
