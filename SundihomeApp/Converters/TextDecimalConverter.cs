using System;
using System.Globalization;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class TextDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                {
                    return "";
                }

                NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;

                string strValue = value.ToString();

                strValue = strValue.Replace(",,", ",").Replace("..", ".");

                decimal decValue = decimal.Parse(strValue);
                string text = decValue.ToString("N", nfi);

                if (text.EndsWith(",00", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Replace(",00", "");
                }

                return text;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //try
            //{
            //    if (value != null && string.IsNullOrWhiteSpace(value.ToString()) == false)
            //    {
            //        NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;
            //        nfi.NumberDecimalDigits = 2;
            //        return decimal.Parse(value.ToString(), nfi);
            //    }
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            return value;
        }
    }
}