using MauiDemo2.Views;
using MauiDemo2.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MauiDemo2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FollowingPage), typeof(FollowingPage));
            Routing.RegisterRoute(nameof(CardPage), typeof(CardPage));
            Routing.RegisterRoute(nameof(RouteDetailsPage), typeof(RouteDetailsPage));
        }
    }

}
