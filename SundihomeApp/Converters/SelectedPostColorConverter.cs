using System;
using System.Collections.Generic;
using System.Globalization;
using SundihomeApi.Entities;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class SelectedPostColorConverter : IValueConverter
    {
        public List<Guid> SelectedPosts { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.DarkKhaki;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
