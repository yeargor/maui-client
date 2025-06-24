using Microsoft.Maui.Controls;
using MauiDemo2.ViewModels;

namespace MauiDemo2.Views
{
    public partial class PhotoPage : ContentPage
    {
        public PhotoPage()
        {
            InitializeComponent();
        }

        public PhotoPage(RegisterPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}