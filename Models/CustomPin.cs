using Microsoft.Maui.Controls.Maps;
using System.Windows.Input;

namespace MauiDemo2.Models;
public class CustomPin : Pin
{
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CustomPin));

    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
    public static readonly BindableProperty OrderOfVisitProperty = BindableProperty.Create(
        nameof(OrderOfVisit),
        typeof(int?),
        typeof(CustomPin));

    public int? OrderOfVisit
    {
        get => (int?)GetValue(OrderOfVisitProperty);
        set => SetValue(OrderOfVisitProperty, value);
    }

    public static readonly BindableProperty LocationBaseIdProperty =
        BindableProperty.Create(nameof(LocationBaseId), typeof(int?), typeof(CustomPin));

    public int? LocationBaseId
    {
        get => (int?)GetValue(LocationBaseIdProperty);
        set => SetValue(LocationBaseIdProperty, value);
    }
}
