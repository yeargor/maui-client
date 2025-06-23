using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MauiDemo2.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float rating)
            {
                int rounded = Math.Max(0, Math.Min(5, (int)Math.Round(rating)));
                var stars = new List<string>();

                for (int i = 0; i < rounded; i++)
                {
                    stars.Add("star_filled.svg");
                }
                for (int i = rounded; i < 5; i++)
                {
                    stars.Add("star_empty.svg");
                }

                return stars;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
