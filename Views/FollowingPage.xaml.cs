namespace MauiDemo2.Views;

using Android.Media;
using CommunityToolkit.Maui.Alerts;
using MauiDemo2.Models;
using MauiDemo2.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Messages;
using Android.Util;
using Point = MauiDemo2.Models.Point;

[QueryProperty(nameof(RouteId), "RouteId")]
public partial class FollowingPage : ContentPage
{
    public string RouteId { get; set; }
    private readonly MapRouteViewModel _viewModel;
    private Polyline userRouteLine;
    private bool _isAnimating;
    private Polyline _mainRouteLine;
    private const string MainRouteColor = "#88BF50";
    private const string AlternateRouteColor = "#FFD700";
    private const string DefaultRouteColor = "#FF0000";
    private const double ProximityThreshold = 0.02;
    private const float NormalWidth = 6;
    private const float HighlightedWidth = 8;
    
    private Point _lastTargetPoint;
    private List<Location> _cachedRouteLocations;
    private Location _lastUserLocation;

    public FollowingPage(MapRouteViewModel viewModel)
    {
        BindingContext = _viewModel = viewModel;
        InitializeComponent();

        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MapRouteViewModel.IsPinDetailsVisible))
            {
                AnimatePopup(_viewModel.IsPinDetailsVisible);
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        await _viewModel.LoadWeatherData();

        if (int.TryParse(RouteId, out int routeId))
        {
            var route = await _viewModel.LoadRouteAsync(routeId);
            DrawRouteLine();

            if (route.Places.Any())
            {
                var firstPlace = route.Places.First();
                _viewModel.StartTrackingUserCommand.Execute(new Location(firstPlace.Point.Latitude, firstPlace.Point.Longitude));
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(firstPlace.Point.Latitude, firstPlace.Point.Longitude),
                    Distance.FromKilometers(2)));
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel.StopTracking();
    }

    private async Task DrawRouteLine()
    {
        var points = _viewModel.PlacesForRoute.Select(p => p.Point).ToList();

        var (routePoints, distance, time) = await _viewModel.GetRoutePointsAsync(points);
        if (routePoints == null || !routePoints.Any())
        {
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _viewModel.TotalDistance = distance;
            _viewModel.TotalTime = time;
        });

        if (_mainRouteLine != null)
        {
            MyMap.MapElements.Remove(_mainRouteLine);
        }

        _mainRouteLine = new Polyline
        {
            StrokeColor = Color.FromArgb(MainRouteColor),
            StrokeWidth = 10
        };

        foreach (var point in routePoints)
        {
            _mainRouteLine.Geopath.Add(point);
        }

        MyMap.MapElements.Clear();
        MyMap.MapElements.Add(_mainRouteLine);

        var centerLat = routePoints.Average(p => p.Latitude);
        var centerLon = routePoints.Average(p => p.Longitude);
        var mapSpan = MapSpan.FromCenterAndRadius(
            new Location(centerLat, centerLon),
            Distance.FromKilometers(1));
        MyMap.MoveToRegion(mapSpan);
    }

    private async void UpdateUserRouteLine(Location userLocation, Location firstPlace)
    {
        if (!_viewModel.IsTrackingEnabled)
        {
            return;
        }

        var targetCheckpoint = _viewModel.PlacesForRoute.FirstOrDefault(p => !p.IsCompleted);
        if (targetCheckpoint == null)
        {
            _viewModel.IsTrackingEnabled = false;
            await MainThread.InvokeOnMainThreadAsync(() => 
            {
                if (userRouteLine != null)
                {
                    MyMap.MapElements.Remove(userRouteLine);
                    userRouteLine = null;
                }
            });
            return;
        }

        bool isNearMainRoute = IsUserNearMainRoute(userLocation);

        bool targetChanged = _lastTargetPoint == null || 
                            _lastTargetPoint.Latitude != targetCheckpoint.Point.Latitude || 
                            _lastTargetPoint.Longitude != targetCheckpoint.Point.Longitude;
                            
        if (_lastUserLocation != null && 
            userLocation.CalculateDistance(_lastUserLocation, DistanceUnits.Kilometers) < 0.02)
        {
            if (userRouteLine != null)
            {
                await UpdateRouteLineStyle(isNearMainRoute);
            }
            return;
        }

        _lastUserLocation = userLocation;

        try
        {
            var routePoints = new List<Point>
            {
                new Point { Latitude = userLocation.Latitude, Longitude = userLocation.Longitude },
                targetCheckpoint.Point
            };

            var (routeLocations, distance, time) = await _viewModel.GetRoutePointsAsync(routePoints);
            if (routeLocations == null || !routeLocations.Any())
            {
                return;
            }

             _cachedRouteLocations = routeLocations;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _viewModel.TotalDistance = distance;
                _viewModel.TotalTime = time;
            });
            await UpdateMapWithRoute(routeLocations, isNearMainRoute);
        }
        catch (Exception ex)
        {
        }
    }

    private async Task UpdateRouteLineStyle(bool isNearMainRoute)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (userRouteLine != null)
            {
                userRouteLine.StrokeColor = isNearMainRoute 
                    ? Color.FromArgb(AlternateRouteColor) 
                    : Color.FromArgb(DefaultRouteColor);
                userRouteLine.StrokeWidth = isNearMainRoute 
                    ? HighlightedWidth 
                    : NormalWidth;
            }
        });
    }

    private async Task UpdateMapWithRoute(List<Location> routeLocations, bool isNearMainRoute)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (userRouteLine == null)
            {
                userRouteLine = new Polyline
                {
                    StrokeColor = isNearMainRoute 
                        ? Color.FromArgb(AlternateRouteColor) 
                        : Color.FromArgb(DefaultRouteColor),
                    StrokeWidth = isNearMainRoute 
                        ? HighlightedWidth 
                        : NormalWidth
                };
                MyMap.MapElements.Add(userRouteLine);
            }
            else
            {
                userRouteLine.StrokeColor = isNearMainRoute 
                    ? Color.FromArgb(AlternateRouteColor) 
                    : Color.FromArgb(DefaultRouteColor);
                userRouteLine.StrokeWidth = isNearMainRoute 
                    ? HighlightedWidth 
                    : NormalWidth;
            }

            userRouteLine.Geopath.Clear();
            foreach (var location in routeLocations)
            {
                userRouteLine.Geopath.Add(location);
            }

            if (routeLocations.Count > 0)
            {
                var centerLat = routeLocations.Average(p => p.Latitude);
                var centerLon = routeLocations.Average(p => p.Longitude);
                var mapSpan = MapSpan.FromCenterAndRadius(
                    new Location(centerLat, centerLon),
                    Distance.FromKilometers(0.5));
                MyMap.MoveToRegion(mapSpan);
            }
        });
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapRouteViewModel.IsTrackingEnabled))
        {
            if (_viewModel.IsTrackingEnabled)
            {
                _viewModel.UserLocationUpdated += UpdateUserRouteLine;

                 if (_cachedRouteLocations != null && _cachedRouteLocations.Any())
                {
                    MainThread.InvokeOnMainThreadAsync(() => UpdateMapWithRoute(_cachedRouteLocations, IsUserNearMainRoute(_lastUserLocation)));
                }
            }
            else
            {
                _viewModel.UserLocationUpdated -= UpdateUserRouteLine;

                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (userRouteLine != null)
                    {
                        MyMap.MapElements.Remove(userRouteLine);
                        userRouteLine = null;
                    }
                });
            }
        }
    }

    public async void AnimatePopup(bool isVisible)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        try
        {
            if (isVisible)
            {
                PinDetailsPopup.IsVisible = true;
                await Task.Delay(50); 
                PinDetailsPopup.TranslationY = this.Height;
                
                var targetY = this.Height / 6 - PinDetailsPopup.Height / 2;
                
                if (targetY < 0) targetY = 0;
                
                await PinDetailsPopup.TranslateTo(0, targetY, 250, Easing.CubicInOut);
            }
            else
            {
                await PinDetailsPopup.TranslateTo(0, this.Height, 250, Easing.CubicInOut);
                PinDetailsPopup.IsVisible = false;
            }
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private bool IsUserNearMainRoute(Location userLocation)
    {
        if (_mainRouteLine == null || _mainRouteLine.Geopath.Count == 0)
            return false;

        foreach (var point in _mainRouteLine.Geopath)
        {
            double distance = userLocation.CalculateDistance(point, DistanceUnits.Kilometers);
            if (distance <= ProximityThreshold)
            {
                return true;
            }
        }
        return false;
    }
}