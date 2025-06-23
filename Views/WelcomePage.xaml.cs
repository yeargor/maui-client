using Microsoft.Maui.Controls;

namespace MauiDemo2.Views
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
