using System;
using System.Globalization;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class FurnitureProductStatusNameConverter : IValueConverter
    {
        public FurnitureProductStatusNameConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                int status = (int)value;
                if (status == 0)
                {
                    return Language.dang_ban_status;
                }
                else
                {
                    return Language.ngung_ban;
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
