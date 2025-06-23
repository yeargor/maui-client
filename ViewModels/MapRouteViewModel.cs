using System.Collections.ObjectModel;
using System.Windows.Input;
using AutoFixture;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Messages;
using MauiDemo2.Models;
using MauiDemo2.Services;
using MauiDemo2.Views;
using Microsoft.Maui.Controls.Maps;
using Point = MauiDemo2.Models.Point;

namespace MauiDemo2.ViewModels
{
    public partial class MapRouteViewModel : ObservableObject
    {
        private RouteFollowingMap? _currentRoute;
        public List<Place> PlacesForRoute { get; } = new();
        public ObservableCollection<LocationPin> LocationPins { get; } = new();
        private readonly RouteService _routeService;
        private readonly PointsService _pointsService;
        public event Action<Location, Location>? UserLocationUpdated;
        public event Action<LocationPin>? CheckpointUpdated;
        private CancellationTokenSource? trackingTokenSource;

        [ObservableProperty]
        private double latitude;
        [ObservableProperty]
        private double longitude;
        [ObservableProperty]
        private bool isListening;
        [ObservableProperty]
        private bool isTrackingEnabled;

        [ObservableProperty]
        private bool isPinDetailsVisible;

        [ObservableProperty]
        private string selectedPinName;

        private readonly WeatherService _weatherService;

        [ObservableProperty]
        private WeatherCurrent weatherData = new();

        private LocationPin? currentLocationPin;
        [ObservableProperty]
        private string currentTabContent;

        [ObservableProperty]
        private LocationBase? selectedLocationBase;
        [ObservableProperty]
        private double _totalDistance;
        [ObservableProperty]
        private Location _lastKnownLocation;
        private double _totalTime;
        public double TotalTime
        {
            get => _totalTime;
            set
            {
                SetProperty(ref _totalTime, value);
                OnPropertyChanged(nameof(FormattedTime));
            }
        }
        public string FormattedTime => $"{Math.Ceiling(TotalTime / 60)} МИН";

        private readonly IFixture _fixture;

        public IAsyncRelayCommand<int> LoadRouteAsyncCommand { get; }

        public MapRouteViewModel(IFixture fixture, RouteService routeService, WeatherService weatherService, PointsService pointsService)
        {
            _routeService = routeService;
            _fixture = fixture;
            _weatherService = weatherService;
            _pointsService = pointsService;
            LoadRouteAsyncCommand = new AsyncRelayCommand<int>(LoadRouteAsync);
            WeakReferenceMessenger.Default.Register<DeviceLocation>(this, (sender, deviceLocation) =>
            {
                Latitude = deviceLocation.Latitude;
                Longitude = deviceLocation.Longitude;
            });

          WeakReferenceMessenger.Default.Register<PinClickedMessage>(this, (r, message) =>
            {
                LocationBase? foundLocationBase = null;

                foundLocationBase = PlacesForRoute.FirstOrDefault(p =>
                    p.Point.Latitude == message.PinLocation.Latitude &&
                    p.Point.Longitude == message.PinLocation.Longitude);

                if (foundLocationBase == null && _currentRoute != null)
                {
                    foundLocationBase = _currentRoute.AdditionalPlaces.FirstOrDefault(p =>
                        p.Point.Latitude == message.PinLocation.Latitude &&
                        p.Point.Longitude == message.PinLocation.Longitude);
                }

                if (foundLocationBase != null)
                {
                    SelectedLocationBase = foundLocationBase;
                    IsPinDetailsVisible = true;
                }
                else
                {
                    SelectedLocationBase = null;
                    IsPinDetailsVisible = false;
                }
            });
        }

        [RelayCommand]
        private void Add()
        {
            var newLocation = _fixture.Build<LocationPin>()
                                     .With(x => x.ImageSource, ImageSource.FromFile("heart.svg"))
                                     .With(x => x.Location, new Location(Random.Shared.Next(44, 51), Random.Shared.Next(23, 40)))
                                     .Create();
            LocationPins.Add(newLocation);
        }

        [RelayCommand]
        private void Remove()
        {
            if (LocationPins.Count > 0)
            {
                LocationPins.RemoveAt(LocationPins.Count - 1);
            }
        }

        [RelayCommand]
        private void RemoveAll()
        {
            LocationPins.Clear();
        }

     public async Task<(List<Location> Points, double Distance, double Time)> GetRoutePointsAsync(List<Point> places)
    {
        string apiKey = "4694ebc07b654d96b095f84d490b17ef";
        string mode = "walk";
        var json = await _pointsService.GetRouteAsync(places, apiKey, mode);

        if (json == null)
        {
            return (new List<Location>(), 0, 0);
        }

        try
        {
            var firstResult = json["results"]?[0];
            if (firstResult == null)
            {
                return (new List<Location>(), 0, 0);
            }

            double distance = (double)firstResult["distance"];
            double time = (double)firstResult["time"];

            var geometry = firstResult["geometry"]; // Получаем массив всех сегментов
            if (geometry == null)
            {
                return (new List<Location>(), distance, time);
            }

            var routePoints = new List<Location>();
            
            // Перебираем все сегменты маршрута
            foreach (var segment in geometry)
            {
                // Перебираем все точки в текущем сегменте
                foreach (var point in segment)
                {
                    try
                    {
                        double lon = (double)point["lon"];
                        double lat = (double)point["lat"];
                        routePoints.Add(new Location(lat, lon));
                    }
                    catch (Exception)
                    {
                        // Пропускаем некорректные точки
                        continue;
                    }
                }
            }

            return (routePoints, distance, time);
        }
        catch (Exception)
        {
            return (new List<Location>(), 0, 0);
        }
    }

        public async Task<RouteFollowingMap> LoadRouteAsync(int routeId)
        {
            _currentRoute = await _routeService.GetRouteForMapPageAsync(routeId);

            PlacesForRoute.Clear();
            LocationPins.Clear();

            foreach (var place in _currentRoute.Places.OrderBy(p => p.OrderOfVisit))
            {
                PlacesForRoute.Add(place);

                var icon = place.IsCompleted
                    ? ImageSource.FromFile("route_point_completed.svg")
                    : ImageSource.FromFile("route_point.svg");

                var newLocation = _fixture.Build<LocationPin>()
                                           .With(x => x.ImageSource, icon)
                                           .With(x => x.Location, new Location(place.Point.Latitude, place.Point.Longitude))
                                           .With(x => x.Description, place.Description)
                                           .With(x => x.OrderOfVisit, place.IsCompleted ? (int?)null : place.OrderOfVisit)
                                           .With(x => x.LocationBaseId, place.Id)
                                           .Create();
                LocationPins.Add(newLocation);
            }

            foreach (var addPlace in _currentRoute.AdditionalPlaces)
            {
                var newLocation = _fixture.Build<LocationPin>()
                                           .With(x => x.ImageSource, ImageSource.FromFile("route_point_additional.svg"))
                                           .With(x => x.Location, new Location(addPlace.Point.Latitude, addPlace.Point.Longitude))
                                           .With(x => x.Description, addPlace.Description)
                                           .With(x => x.OrderOfVisit, (int?)null)
                                           .With(x => x.LocationBaseId, addPlace.Id)
                                           .Create();
                LocationPins.Add(newLocation);
            }

            var firstUncompletedPlace = PlacesForRoute.FirstOrDefault(p => !p.IsCompleted);
            if (firstUncompletedPlace != null)
            {
                StartTrackingUserCommand.Execute(new Location(firstUncompletedPlace.Point.Latitude, firstUncompletedPlace.Point.Longitude));
            }

            return _currentRoute;
        }

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task RealTimeLocationTracker(CancellationToken cancellationToken)
        {
            var progress = new Progress<Location>(location =>
            {
                if (currentLocationPin is null)
                {
                    currentLocationPin = new LocationPin
                    {
                        ImageSource = ImageSource.FromFile("user_cursor.svg"),
                        Location = location,
                        Description = "I am here!"
                    };
                }
                else
                {
                    LocationPins.Remove(currentLocationPin);
                    currentLocationPin.Location = location;
                }

                LocationPins.Add(currentLocationPin);
            });
            await Geolocator.Default.StartListening(progress, cancellationToken);
        }

        [RelayCommand]
        private async Task NavigateToCardPage()
        {
            await Shell.Current.GoToAsync(nameof(CardPage));
        }

        [RelayCommand]
        private async Task StartTrackingUser(Location firstPlace)
        {
            if (!IsTrackingEnabled)
            {
                return;
            }

            trackingTokenSource?.Cancel();
            trackingTokenSource = new CancellationTokenSource();
            var token = trackingTokenSource.Token;

            await Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var userLocation = await Geolocation.GetLastKnownLocationAsync();
                    if (userLocation == null) continue;

                    var distance = Location.CalculateDistance(userLocation, firstPlace, DistanceUnits.Kilometers);

                    if (distance <= 0.05)
                    {
                        MarkCheckpointAsCompleted(firstPlace);

                        var nextCheckpoint = PlacesForRoute.FirstOrDefault(p => !p.IsCompleted);
                        if (nextCheckpoint != null)
                        {
                            trackingTokenSource?.Cancel();
                            await Task.Delay(500);
                            await StartTrackingUser(new Location(nextCheckpoint.Point.Latitude, nextCheckpoint.Point.Longitude));
                        }
                        else
                        {
                            MarkCheckpointAsCompleted(firstPlace);
                        }
                    }
                    UserLocationUpdated?.Invoke(userLocation, firstPlace);
                    await Task.Delay(2000);
                }
            }, token);
        }

        private void SaveRouteState()
        {
            var currentRoute = new RouteFollowingMap
            {
                Id = PlacesForRoute.FirstOrDefault()?.Id ?? 0,
                Name = "Current Route",
                Places = PlacesForRoute.ToList(),
                AdditionalPlaces = new List<AdditionalPlace>()
            };

            _routeService.SaveRouteState(currentRoute);
        }

        private void MarkCheckpointAsCompleted(Location completedPoint)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                var completedPlace = PlacesForRoute.FirstOrDefault(p =>
                    p.Point != null &&
                    p.Point.Latitude == completedPoint.Latitude &&
                    p.Point.Longitude == completedPoint.Longitude);

                if (completedPlace != null)
                {
                    completedPlace.IsCompleted = true;

                    var originalPlace = _routeService.GetRouteForMapPageAsync(completedPlace.Id).Result?.Places
                        .FirstOrDefault(p => p.Id == completedPlace.Id);
                    if (originalPlace != null)
                    {
                        originalPlace.IsCompleted = true;
                    }

                    var nextCheckpoint = PlacesForRoute.FirstOrDefault(p => !p.IsCompleted) ?? completedPlace;
                    SaveRouteState();
                }

                var completedPin = LocationPins.FirstOrDefault(p =>
                    p.Location != null &&
                    p.Location.Latitude == completedPoint.Latitude &&
                    p.Location.Longitude == completedPoint.Longitude);

                if (completedPin != null)
                {
                    completedPin.ImageSource = ImageSource.FromFile("route_point_completed.svg");
                    OnPropertyChanged(nameof(LocationPins));
                    WeakReferenceMessenger.Default.Send(completedPin);
                }
            });
        }

        private void SelectNextCheckpoint()
        {
            var remainingCheckpoints = PlacesForRoute
                .Where(p => !p.IsCompleted)
                .OrderBy(p => p.OrderOfVisit)
                .ToList();

            if (remainingCheckpoints.Any())
            {
                var nextCheckpoint = remainingCheckpoints.First();
                StartTrackingUser(new Location(nextCheckpoint.Point.Latitude, nextCheckpoint.Point.Longitude));
            }
        }
        public void StopTracking()
        {
            trackingTokenSource?.Cancel();
            IsListening = false;
        }
        
        public async Task LoadWeatherData()
        {
            try
            {
                var weather = await _weatherService.GetWeatherAsync();

                if (weather != null)
                {
                    WeatherData = weather;
                    OnPropertyChanged(nameof(WeatherData));
                }
            }
            catch (Exception)
            {
            }
        }

        [RelayCommand]
        private void ToggleTracking()
        {
            IsTrackingEnabled = !IsTrackingEnabled;

            if (!IsTrackingEnabled)
            {
                StopTracking();
            }
            else
            {
                if (IsTrackingEnabled && PlacesForRoute.Any())
                {
                    var firstUncompletedPlace = PlacesForRoute.FirstOrDefault(p => !p.IsCompleted);
                    if (firstUncompletedPlace != null)
                    {
                        StartTrackingUserCommand.Execute(new Location(firstUncompletedPlace.Point.Latitude, firstUncompletedPlace.Point.Longitude));
                    }
                }
            }
        }

        [RelayCommand]
        private void ClosePinDetails()
        {
            IsPinDetailsVisible = false;
            WeakReferenceMessenger.Default.Send(new PopupAnimationMessage(false));
        }
    }
}
