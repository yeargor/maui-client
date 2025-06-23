using System;
using System.Globalization;
using MauiDemo2.Models.Common;

namespace MauiDemo2.Converters
{
    public class FavoriteToHeartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine($"Converter received value: {value}");

            if (value is UserLike userLike)
            {
                return userLike.IsUserFavorite ? "heartred.svg" : "heart.svg";
            }

            return "heart.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
