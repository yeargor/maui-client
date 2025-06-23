using MauiDemo2.Services;
using Microsoft.Maui.Controls.Handlers.Compatibility;

namespace MauiDemo2
{
    public partial class App : Application
    {
        private readonly AuthService _authService = Services.ServiceHelper.GetService<AuthService>();
        private readonly CurrentUserService _currentUserService = Services.ServiceHelper.GetService<CurrentUserService>();

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            AttemptAutoLogin();
        }

        private async void AttemptAutoLogin()
        {
            var email = Preferences.Get("auth_email", null);
            var password = Preferences.Get("auth_password", null);
            System.Diagnostics.Debug.WriteLine($"[AutoLogin] Email: {email}, Password: {(string.IsNullOrEmpty(password) ? "(empty)" : "(hidden)")}");
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                System.Diagnostics.Debug.WriteLine("[AutoLogin] No credentials found. Showing welcome page.");
                await HideTabBarAndShowWelcome();
                return;
            }
            var (success, error) = await _authService.LoginAndSetCurrentUserAsync(email, password);
            System.Diagnostics.Debug.WriteLine($"[AutoLogin] Login result: success={success}, error={error}");
            if (!success)
            {
                System.Diagnostics.Debug.WriteLine("[AutoLogin] Login failed. Showing welcome page.");
                await HideTabBarAndShowWelcome();
            }
        }

        private async Task HideTabBarAndShowWelcome()
        {
            if (MainPage is Shell shell)
            {
                var tabBar = shell.Items.FirstOrDefault();
                if (tabBar != null)
                    tabBar.SetValue(Shell.TabBarIsVisibleProperty, false);
            }
            await MainPage.Navigation.PushModalAsync(new MauiDemo2.Views.WelcomePage());
        }
    }
}