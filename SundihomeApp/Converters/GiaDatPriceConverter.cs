using System;
using System.Globalization;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class GiaDatPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is decimal)
                {
                    var decValue = (decimal)value * 1000;
                    return DecimalHelper.ToCurrency(decValue) + " đ/m2";
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
