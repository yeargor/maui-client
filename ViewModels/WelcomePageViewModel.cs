using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDemo2.Views;
using MauiDemo2.Services;
using System.Threading.Tasks;
using System.Linq;

namespace MauiDemo2.ViewModels
{
    public partial class WelcomePageViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task ToLoginPage()
        {
            var loginPage = new LoginPage();
            await Application.Current.MainPage.Navigation.PushModalAsync(loginPage);
        }

         [RelayCommand]
        private async Task ToRegisterPage()
        {
            var registerPage = new RegisterPage();
            await Application.Current.MainPage.Navigation.PushModalAsync(registerPage);
        }
    }
}
