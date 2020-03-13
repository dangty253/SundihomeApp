using System;
using System.Globalization;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class LiquidationTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is string)
            {
                if (((string)value) == "Cần thanh lý")
                {
                    return Language.can_thanh_ly;
                }
                else if ((string)value == "Cần mua")
                {
                    return Language.can_mua;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
