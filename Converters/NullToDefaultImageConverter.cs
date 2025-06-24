using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiDemo2.Converters
{
    public class NullToDefaultImageConverter : IValueConverter
    {
        public string DefaultImage { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            if (value == null)
                return DefaultImage;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
