using MauiDemo2.ViewModel;

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
            FlyoutMenu.TranslationX = 300;
            FlyoutMenu.IsVisible = true;
            await FlyoutMenu.TranslateTo(0, 0, 300, Easing.CubicInOut);
        }
        else
        {
            await FlyoutMenu.TranslateTo(300, 0, 300, Easing.CubicInOut);
            FlyoutMenu.IsVisible = false;
        }
    }
    private void OnTestTapped(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Tapped event received!");
    }

}