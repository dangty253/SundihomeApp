using System;
using System.Globalization;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class MonthToYearConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var month = (int)value;
                if (month == 0)
                    return "Không";
                if (month % 12 == 0)
                {
                    var year = (int)month / 12;
                    return year.ToString() + " năm";
                }
                return month.ToString() + " tháng";
            }
            return "Không";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
