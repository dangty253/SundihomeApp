using System;
using System.Globalization;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class NullToHideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            //if (value is string && string.IsNullOrEmpty(((string)value)))
            //{
            //    return false;
            //}

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
