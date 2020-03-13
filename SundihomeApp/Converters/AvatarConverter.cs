using System;
using System.Globalization;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class AvatarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string avatar = value.ToString();
            if (avatar.StartsWith("avatar/", StringComparison.OrdinalIgnoreCase))
            {
                return Configuration.ApiConfig.CloudStorageApiCDN + "/" + avatar;
            }
            else
            {
                return avatar;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
