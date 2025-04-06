using MauiDemo2.Views;

namespace MauiDemo2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {

            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        }
    }
}
