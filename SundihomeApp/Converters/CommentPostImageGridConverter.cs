using System;
using System.Globalization;
using System.Linq;
using FFImageLoading.Forms;
using Stormlion.PhotoBrowser;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class CommentPostImageGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string[] Images = (string[])value;
            var Photos = Images.Select(x => new Photo() { Title = "", URL = Configuration.ApiConfig.CloudStorageApiCDN + "/post/" + x }).ToList();

            var grid = new Grid()
            {
                ColumnSpacing = 2,
                RowSpacing = 2,
            };

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < 3; i++)
            {
                string imageSource = Configuration.ApiConfig.CloudStorageApiCDN + "/post/" + Images[i];
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
            return grid;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
