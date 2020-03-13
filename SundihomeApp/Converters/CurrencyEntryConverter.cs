using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;


namespace SundihomeApp.Converters
{
    public class CurrencyEntryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || (decimal)value == 0) return "";
                return String.Format("{0:#,#}", (decimal)value);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    return decimal.Parse(value.ToString().Replace(".", ""));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}