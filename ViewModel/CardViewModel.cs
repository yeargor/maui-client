using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDemo2.Dtos;
using MauiDemo2.Models;
using MauiDemo2.Services;
using MauiDemo2.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiDemo2.ViewModel
{
    public partial class CardViewModel : ObservableObject
    {
        public ObservableCollection<RouteDto> Routes { get; set; } = new ObservableCollection<RouteDto>();

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string message;

        public IAsyncRelayCommand GetRoutesCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenUserPageCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenRouteFollowCommand { get; }
        public ICommand OpenNotificationsCommand { get; }
        public ICommand OpenRouteDetailsCommand { get; }

        public CardViewModel()
        {
            GetRoutesCommand = new AsyncRelayCommand(GetRoutes);
            ToggleFavoriteCommand = new RelayCommand<RouteDto>(ToggleFavorite);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenUserPageCommand = new RelayCommand(OpenUserPage);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenRouteFollowCommand = new RelayCommand(OpenRouteFollow);
            OpenNotificationsCommand = new RelayCommand(OpenNotifications);
            OpenRouteDetailsCommand = new RelayCommand<RouteDto>(OpenRouteDetails);

            _ = GetRoutes();
        }

        private async Task GetRoutes()
        {
            IsBusy = true;
            await RouteServiceCall<ObservableCollection<RouteDto>>.Get("cards", RoutesDataLoaded, RoutesDataLoadFailed);
            IsBusy = false;
        }

        private void RoutesDataLoaded(ObservableCollection<RouteDto> routeList)
        {
            Routes.Clear();
            foreach (var route in routeList)
            {
                Routes.Add(route);
            }
            Message = "Маршруты загружены";
        }

        private void RoutesDataLoadFailed(Exception exception)
        {
            Message = exception?.Message;
        }

        private void ToggleFavorite(RouteDto route)
        {
            route.IsFavorite = !route.IsFavorite;
            // Здесь будет API-запрос для обновления избранного
        }

        private async void OpenProfile() => await Shell.Current.GoToAsync(nameof(ProfilePage));

        private void OpenUserPage() => Console.WriteLine("Открываем страницу пользователя");

        private void OpenSettings() => Console.WriteLine("Открываем настройки");

        private void OpenNotifications() => Console.WriteLine("Открываем уведомления");

        private void OpenRouteFollow() => Console.WriteLine("Открываем уведомления");

        private void OpenRouteDetails(RouteDto route) => Console.WriteLine($"Открываем страницу маршрута {route.Name}");
    }
}   