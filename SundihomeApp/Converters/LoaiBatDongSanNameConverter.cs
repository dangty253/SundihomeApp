using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;


namespace SundihomeApp.Converters
{
    public class LoaiBatDongSanNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            int id = (int)value;
            Models.LoaiBatDongSanModel loai = Models.LoaiBatDongSanModel.GetList(null).SingleOrDefault(x => x.Id == id);
            if (loai != null)
            {
                return loai.Name;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}