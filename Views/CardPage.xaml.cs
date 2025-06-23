using MauiDemo2.Models;
using MauiDemo2.ViewModel;
using MauiDemo2.ViewModels;

namespace MauiDemo2.Views;

public partial class CardPage : ContentPage
{
    public CardPage(CardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    public async void FlyoutAnimation(bool isVisible)
    {
        if (isVisible)
        {
            await ShowDimOverlayAsync();
            FlyoutMenu.TranslationX = 300;
            FlyoutMenu.IsVisible = true;
            await FlyoutMenu.TranslateTo(0, 0, 300, Easing.CubicInOut);
        }
        else
        {
            await FlyoutMenu.TranslateTo(300, 0, 300, Easing.CubicInOut);
            FlyoutMenu.IsVisible = false;
            await HideDimOverlayAsync();
        }
    }

    private async Task ShowDimOverlayAsync()
    {
        DimOverlay.Opacity = 0;
        DimOverlay.IsVisible = true;
        await DimOverlay.FadeTo(0.5, 300, Easing.CubicOut);
    }
    private async Task HideDimOverlayAsync()
    {
        await DimOverlay.FadeTo(0, 300, Easing.CubicIn);
        DimOverlay.IsVisible = false;
    }
}
