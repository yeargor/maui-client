using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Converters
{
    public class ActiveTabConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activeTab = value as string;
            var tabParameter = parameter as string;

            if (activeTab == tabParameter)
            {
                return $"{tabParameter}_colored.svg";
            }
            return $"{tabParameter}.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
