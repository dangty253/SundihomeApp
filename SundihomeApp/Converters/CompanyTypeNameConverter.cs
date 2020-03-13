using System;
using System.Globalization;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class CompanyTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            short id = (short)value;

            LoaiCongTyModel loaiCongTyModel = LoaiCongTyData.GetById(id);
            return loaiCongTyModel.Name;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
