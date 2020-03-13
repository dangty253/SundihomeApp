using System;
using System.Globalization;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class GoiVayMaxTimeConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                var unit = (int)value;
                if (unit == 0)
                {
                    return " " + Language.year.ToLower();
                }
                else if (unit == 1)
                {
                    return " " + Language.month.ToLower();
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
