using System;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace MauiDemo2.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string textParams)
            {
                var texts = textParams.Split('|');
                return boolValue ? texts[1] : texts[0];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}