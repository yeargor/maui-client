using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Dtos;
using MauiDemo2.Messages;
using MauiDemo2.Models;
using MauiDemo2.Models.Common;
using MauiDemo2.Models.Route;
using MauiDemo2.Services;
using MauiDemo2.Views;
using MauiDemo2.WebClients;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiDemo2.ViewModels
{
    public partial class CardViewModel : ObservableObject
    {
        private ObservableCollection<RouteCardResponseDto> _routes = new ObservableCollection<RouteCardResponseDto>();
        public ObservableCollection<RouteCardResponseDto> Routes
        {
            get => _routes;
            set
            {
                _routes = value;
                OnPropertyChanged(nameof(Routes));
            }
        }

        private string _searchText = String.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FilterRoutes();
                }
            }
        }

        private ObservableCollection<RouteCardResponseDto> _filteredRoutes = new();
        public ObservableCollection<RouteCardResponseDto> FilteredRoutes
        {
            get => _filteredRoutes;
            set
            {
                _filteredRoutes = value;
                OnPropertyChanged(nameof(FilteredRoutes));
            }
        }

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isFlyoutVisible;

        private readonly RouteService _routeService;

        public IAsyncRelayCommand GetRoutesCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenUserPageCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenRouteFollowCommand { get; }
        public ICommand OpenNotificationsCommand { get; }
        public ICommand OpenRouteDetailsCommand { get; }
        public ICommand ToggleFlyoutCommand { get; }
        public ICommand UpdateFavoriteCommand => new RelayCommand<RouteCardResponseDto>((route) => UpdateFavorite(route));
        public ICommand OpenRouteFollowingCommand => new RelayCommand<RouteCardResponseDto>(OpenRouteFollowingPage);
        public ICommand StartRouteCommand { get; }

        public CardViewModel(RouteService routeService)
        {
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));

            GetRoutesCommand = new AsyncRelayCommand(LoadRoutesAsync);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenUserPageCommand = new RelayCommand(OpenUserPage);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenRouteFollowCommand = new RelayCommand(OpenRouteFollow);
            OpenNotificationsCommand = new RelayCommand(OpenNotifications);
            OpenRouteDetailsCommand = new RelayCommand<RouteCardResponseDto>(OpenRouteDetails);
            ToggleFlyoutCommand = new RelayCommand(ToggleFlyout);
            StartRouteCommand = new Command(StartRoute);
            // _routeService.RouteUpdated += OnRouteUpdated;
            WeakReferenceMessenger.Default.Register<RouteUpdatedMessage>(this, (r, msg) =>
            {
                var (routeId, userLike) = msg.Value;
                OnRouteUpdated(routeId, userLike);
            });

            _ = LoadRoutesAsync();
        }
        partial void OnIsFlyoutVisibleChanged(bool value)
        {
            Page currentPage = Shell.Current?.CurrentPage;
            if (currentPage is CardPage cardPage)
            {
                cardPage.FlyoutAnimation(value);
            }
            else if (currentPage is NavigationPage navPage && navPage.CurrentPage is CardPage cardPage2)
            {
                cardPage2.FlyoutAnimation(value);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CurrentPage is not CardPage. Actual type: " + currentPage?.GetType().ToString());
            }
        }
        private async void OpenRouteFollowingPage(RouteCardResponseDto route)
        {
            if (route != null)
            {
                await Shell.Current.GoToAsync($"FollowingPage?RouteId={route.RouteInfo.RouteId}");
            }
        }
        
        [RelayCommand]
        private async Task OpenRouteFromCard(RouteCardResponseDto route)
        {
            await Shell.Current.GoToAsync($"{nameof(RouteDetailsPage)}?RouteId={route.RouteInfo.RouteId}");
        }

        private void ToggleFlyout()
        {
            System.Diagnostics.Debug.WriteLine("ToggleFlyout command fired");
            IsFlyoutVisible = !IsFlyoutVisible;
        }
        private async Task LoadRoutesAsync()
        {
            IsBusy = true;
            var routes = await _routeService.GetRoutesAsync();

            Routes.Clear();
            foreach (var route in routes)
            {
                Routes.Add(route);
            }
            FilteredRoutes = new ObservableCollection<RouteCardResponseDto>(Routes);
            IsBusy = false;
        }

        private void OnRouteUpdated(int routeId, UserLike? userLike)
        {
            System.Diagnostics.Debug.WriteLine($"[CardViewModel] OnRouteUpdated: RouteId={routeId}, UserLike={(userLike != null ? userLike.IsUserFavorite.ToString() : "null")}");
            var route = Routes.FirstOrDefault(r => r.RouteInfo.RouteId == routeId);
            if (route != null)
            {
                route.RouteInfo.UserLike = userLike;
                OnPropertyChanged(nameof(Routes));
                System.Diagnostics.Debug.WriteLine($"[CardViewModel] OnRouteUpdated: Route {route.RouteInfo.RouteTitle}, UserLike={(userLike != null ? userLike.IsUserFavorite.ToString() : "null")}");
            }
        }

        public async void UpdateFavorite(RouteCardResponseDto route)
        {
            if (route != null)
            {
                System.Diagnostics.Debug.WriteLine($"[CardViewModel] UpdateFavorite: BEFORE call, RouteId={route.RouteInfo.RouteId}, UserLike={(route.RouteInfo.UserLike != null ? route.RouteInfo.UserLike.IsUserFavorite.ToString() : "null")}");
                await _routeService.Update(route.RouteInfo.RouteId);
                System.Diagnostics.Debug.WriteLine($"[CardViewModel] UpdateFavorite: AFTER call, RouteId={route.RouteInfo.RouteId}, UserLike={(route.RouteInfo.UserLike != null ? route.RouteInfo.UserLike.IsUserFavorite.ToString() : "null")}");
                Routes = new ObservableCollection<RouteCardResponseDto>(Routes);
            }
        }

        private void OpenRouteDetails(RouteCardResponseDto route) => System.Diagnostics.Debug.WriteLine($"Открываем детали маршрута {route.RouteInfo.RouteTitle}");

        private async void StartRoute()
        {
            await Shell.Current.GoToAsync(nameof(FollowingPage));
        }

        private void FilterRoutes()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredRoutes = new ObservableCollection<RouteCardResponseDto>(Routes);
            }
            else
            {
                var filtered = Routes.Where(r => r.RouteInfo.RouteTitle != null && r.RouteInfo.RouteTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                FilteredRoutes = new ObservableCollection<RouteCardResponseDto>(filtered);
            }
        }

        private void OpenProfile()
        {
            Shell.Current.GoToAsync(nameof(ProfilePage));
        }

        private void OpenUserPage()
        {

        }

        private void OpenSettings()
        {

        }

        private void OpenRouteFollow()
        {
 
        }

        private void OpenNotifications()
        {
        }
    }
}