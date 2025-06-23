using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiDemo2.Converters
{
    public class DateToDaysAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime createdAt)
            {
                var days = (DateTime.Now.Date - createdAt.Date).Days;
                if (days < 0) days = 0;
                if (days == 0) return "сегодня";
                if (days == 1) return "1 день назад";
                if (days >= 2 && days <= 4) return $"{days} дня назад";
                return $"{days} дней назад";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
