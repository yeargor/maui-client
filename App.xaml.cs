using Microsoft.Maui.Controls.Handlers.Compatibility;

namespace MauiDemo2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}