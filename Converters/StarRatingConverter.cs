using System.Globalization;

namespace MauiDemo2.Converters;
public class StarRatingConverter : IValueConverter, IMarkupExtension
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value приходит как Int32, но это индекс звезды (1..5), а не цифра для отображения
        if (value is int starIndex && parameter != null)
        {
            float selectedRating = System.Convert.ToSingle(parameter, CultureInfo.InvariantCulture);
            // Индексы в x:Array идут с 1, поэтому сравниваем напрямую
            return starIndex <= selectedRating ? "star_filled.svg" : "star_empty.svg";
        }
        return "star_empty.svg";
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}