using MauiDemo2.ViewModel;

namespace MauiDemo2.Views;

public partial class CardPage : ContentPage
{
    public CardPage(CardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}