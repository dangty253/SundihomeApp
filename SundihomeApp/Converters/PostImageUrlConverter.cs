using System;
using System.Globalization;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class PostImageUrlConverter : IValueConverter
    {
        public string Folder { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == "") return null;
            return ImageHelper.GetImageUrl(Folder, value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return value;
        }
    }
}
