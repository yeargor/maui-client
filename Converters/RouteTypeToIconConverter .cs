using System.Globalization;

namespace MauiDemo2.Converters
{
    public class RouteTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RouteType)value switch
            {
                RouteType.Walking => "bywalking.svg",
                RouteType.Cycling => "bybicycle.svg",
                RouteType.Car => "bycar.svg"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}