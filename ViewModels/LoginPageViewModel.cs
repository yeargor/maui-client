using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDemo2.Views;
using MauiDemo2.Services;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Maui.Storage;
using System.Text;
using System.Text.Json;

namespace MauiDemo2.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private bool isPassword = true;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string eyeIcon = "eyeoff.svg";

        [ObservableProperty]
        private string? loginError;

        [ObservableProperty]
        private bool isBusy;

        private readonly AuthService _authService = ServiceHelper.GetService<AuthService>();

        public LoginPageViewModel()
        {
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPassword = !IsPassword;
            EyeIcon = IsPassword ? "eyeoff.svg" : "eyeon.svg";
        }

        [RelayCommand]
        private async Task ToWelcomePage()
        {
            var welcomePage = new WelcomePage();
            if (Application.Current?.MainPage?.Navigation != null)
                await Application.Current.MainPage.Navigation.PushModalAsync(welcomePage);
        }

        [RelayCommand]
        private async Task ToRegisterPage()
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                await Application.Current.MainPage.Navigation.PushModalAsync(new MauiDemo2.Views.RegisterPage());
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            IsBusy = true;
            LoginError = null;
            System.Diagnostics.Debug.WriteLine($"[Login] Email: {Email}, Password: {(string.IsNullOrEmpty(Password) ? "(empty)" : "(hidden)")}");
            var (success, error) = await _authService.LoginAndSetCurrentUserAsync(Email, Password);
            System.Diagnostics.Debug.WriteLine($"[Login] Result: success={success}, error={error}");
            if (success)
            {
                IsBusy = false;
                Application.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync(nameof(CardPage));
                return;
            }
            else
            {
                LoginError = error;
            }
            IsBusy = false;
            if (!string.IsNullOrEmpty(LoginError) && Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка входа", LoginError, "OK");
            }
        }
    }
}
