using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDemo2.Dtos;
using MauiDemo2.Models;
using MauiDemo2.Views;
using MauiDemo2.WebClients;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiDemo2.ViewModel
{
    public partial class CardViewModel : ObservableObject
    {
        public ObservableCollection<RouteMainPage> Routes { get; set; } = new ObservableCollection<RouteMainPage>();

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

        private readonly IMapper _mapper;
        private readonly string lorem = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

        public CardViewModel(IMapper mapper)
        {
            GetRoutesCommand = new AsyncRelayCommand(GetRoutes);
            ToggleFavoriteCommand = new RelayCommand<RouteMainPage>(ToggleFavorite);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenUserPageCommand = new RelayCommand(OpenUserPage);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenRouteFollowCommand = new RelayCommand(OpenRouteFollow);
            OpenNotificationsCommand = new RelayCommand(OpenNotifications);
            OpenRouteDetailsCommand = new RelayCommand<RouteMainPage>(OpenRouteDetails);
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _ = GetRoutes();
        }

        private async Task GetRoutes()
        {
            IsBusy = true;
            var testRoutes = GenerateTestRoutes();

            Routes.Clear();
            foreach (var route in testRoutes)
            {
                Routes.Add(route);
            }
            //await RouteServiceClient<ObservableCollection<TripResponseDto>>.Get("routes", RoutesDataLoaded, RoutesDataLoadFailed);
            IsBusy = false;
        }

        private void RoutesDataLoaded(ObservableCollection<TripResponseDto> dtoList)
        {
            Routes.Clear();

            foreach (var dto in dtoList)
            {
                var route = _mapper.Map<RouteMainPage>(dto);
                Routes.Add(route);
            }

            Message = "Маршруты загружены";
        }

        private void RoutesDataLoadFailed(Exception exception)
        {
            Console.WriteLine(exception?.Message);
        }

        private List<RouteMainPage> GenerateTestRoutes()
        {
            return new List<RouteMainPage>
            {
                new RouteMainPage
                {
                    Id = 1,
                    Name = "Горный маршрут",
                    Description = lorem,
                    TimesCompleted = 42,
                    Rating = 4.8,
                    Distance = 12.5,
                    DaysAgo = "2 дня назад",
                    IsFavorite = true
                },
                new RouteMainPage
                {
                    Id = 2,
                    Name = "Лесная тропа",
                    Description = lorem,
                    TimesCompleted = 28,
                    Rating = 4.5,
                    Distance = 8.2,
                    DaysAgo = "5 дней назад",
                    IsFavorite = false
                },
                new RouteMainPage
                {
                    Id = 3,
                    Name = "Озерный круг",
                    Description = lorem,
                    TimesCompleted = 35,
                    Rating = 4.7,
                    Distance = 10.0,
                    DaysAgo = "1 день назад",
                    IsFavorite = true
                },
                new RouteMainPage
                {
                    Id = 4,
                    Name = "Городской тур",
                    Description = lorem,
                    TimesCompleted = 19,
                    Rating = 4.3,
                    Distance = 6.5,
                    DaysAgo = "3 дня назад",
                    IsFavorite = false
                },
                new RouteMainPage
                {
                    Id = 5,
                    Name = "Речная прогулка",
                    Description = lorem,
                    TimesCompleted = 31,
                    Rating = 4.6,
                    Distance = 9.3,
                    DaysAgo = "1 неделю назад",
                    IsFavorite = true
                }
            };
        }

        private void ToggleFavorite(RouteMainPage route)
        {
            if (route != null)
            {
                route.IsFavorite = !route.IsFavorite;
                // Здесь можно добавить вызов API для обновления избранного на сервере
                Console.WriteLine($"Route {route.Name} favorite status changed to {route.IsFavorite}");
            }
        }
        private async void OpenProfile() => await Shell.Current.GoToAsync(nameof(ProfilePage));

        private void OpenUserPage() => Console.WriteLine("Открываем страницу пользователя");

        private void OpenSettings() => Console.WriteLine("Открываем настройки");

        private void OpenNotifications() => Console.WriteLine("Открываем уведомления");

        private void OpenRouteFollow() => Console.WriteLine("Открываем уведомления");

        private void OpenRouteDetails(RouteMainPage route) => Console.WriteLine($"Открываем страницу маршрута {route.Name}");
    }
}   