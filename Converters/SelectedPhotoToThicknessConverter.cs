using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiDemo2.Converters
{
    public class SelectedPhotoToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = value as string;
            var current = parameter as string;
            if (!string.IsNullOrEmpty(selected) && selected == current)
                return 4; // Selected border thickness
            return 0; // No border if not selected
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedPhotoToStrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = value as string;
            var current = parameter as string;
            if (!string.IsNullOrEmpty(selected) && selected == current)
                return "#F17256";
            return "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
