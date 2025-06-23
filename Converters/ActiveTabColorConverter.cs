using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Converters
{
    public class ActiveTabColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activeTab = value as string;
            var tabParameter = parameter as string;

            if (activeTab == tabParameter)
            {
                return Color.FromArgb("#FE8A70");
            }

            return Color.FromArgb("#343330");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
