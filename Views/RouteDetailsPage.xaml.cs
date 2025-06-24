using MauiDemo2.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MauiDemo2.Views
{
    [QueryProperty(nameof(RouteId), "RouteId")]
    public partial class RouteDetailsPage : ContentPage
    {
        public string? RouteId { get; set; }
        private readonly RouteDetailsViewModel _viewModel;
        private Polyline? _mainRouteLine;
        private const string MainRouteColor = "#88BF50";
        private bool _isMapReady;

        public RouteDetailsPage(RouteDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (int.TryParse(RouteId, out int routeId))
            {
                var route = await _viewModel.LoadRouteAsync(routeId);
                await DrawRouteLine();
            }
        }

        private async Task DrawRouteLine()
        {
            var points = _viewModel.PlacesForRoute;

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
                RouteMap.MapElements.Remove(_mainRouteLine);
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

            RouteMap.MapElements.Clear();
            RouteMap.MapElements.Add(_mainRouteLine);

            var centerLat = routePoints.Average(p => p.Latitude);
            var centerLon = routePoints.Average(p => p.Longitude);

            var paddingFactor = 0.8;
            var offsetRatio = 0.11;

            var tempSpan = MapSpan.FromCenterAndRadius(
                new Location(centerLat, centerLon),
                Distance.FromKilometers(1));

            var offsetLat = tempSpan.LatitudeDegrees * offsetRatio;
            var adjustedCenter = new Location(centerLat + offsetLat, centerLon);

            var minLat = routePoints.Min(p => p.Latitude);
            var maxLat = routePoints.Max(p => p.Latitude);
            var minLon = routePoints.Min(p => p.Longitude);
            var maxLon = routePoints.Max(p => p.Longitude);

            var latDistance = (maxLat - minLat) * paddingFactor * (1 + offsetRatio / 2);
            var lonDistance = (maxLon - minLon) * paddingFactor;
            var maxDistance = Math.Max(latDistance, lonDistance);

            var distanceInKm = maxDistance * 111;

            distanceInKm = Math.Max(0.1, Math.Min(10, distanceInKm));

            var mapSpan = MapSpan.FromCenterAndRadius(
                adjustedCenter,
                Distance.FromKilometers(distanceInKm));

            RouteMap.MoveToRegion(mapSpan);
        }
    }
}