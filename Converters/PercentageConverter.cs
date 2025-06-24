 using System;
using System.Globalization;

namespace MauiDemo2.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height && parameter is string percentParam)
            {
                if (double.TryParse(percentParam, NumberStyles.Any, culture, out double percentage))
                {
                    return height * percentage;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}