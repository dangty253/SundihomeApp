using System;
using System.Globalization;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class NotificationIsReadColorConverter : IValueConverter
    {
        public object Convert(object isRead, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)isRead)
            {
                return Color.White;
            }
            else
            {
                return Color.FromHex("#eeeeee");

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
