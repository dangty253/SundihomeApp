using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SundihomeApp.Converters
{
    public class ContactGroupNameConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            int id = (int)value;
            Models.ContactGroupModel group = Models.ContactGroupModel.GetList().SingleOrDefault(x => x.Id == id);
            if (group != null)
            {
                return group.Name;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
