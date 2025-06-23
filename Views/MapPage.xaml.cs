using MauiDemo2.ViewModels;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls;

namespace MauiDemo2.Views
{
    public partial class MapPage : ContentPage
    {
        private bool _isEditingStarted = false;

        private Frame _circle;

        public MapPage(CreatingRouteViewModel mapPageViewModel)
        {
            InitializeComponent();
            BindingContext = mapPageViewModel;

            mapPageViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(CreatingRouteViewModel.Latitude) ||
                    e.PropertyName == nameof(CreatingRouteViewModel.Longitude))
                {
                    UpdateMapPosition(mapPageViewModel.Latitude, mapPageViewModel.Longitude);
                }
            };
        }

        private void UpdateMapPosition(double latitude, double longitude)
        {
            var position = new Location(latitude, longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
        }

        private void OnMapTapped(object sender, TappedEventArgs e)
        {
            if (_isEditingStarted) return;

            Point? tapPointNullable = e.GetPosition(tapOverlay);
            if (!tapPointNullable.HasValue) return;

            Point tapPoint = tapPointNullable.Value;

            if (_circle == null)
            {
                CreateCircle(tapPoint);
            }
            else
            {
                Rect circleBounds = AbsoluteLayout.GetLayoutBounds(_circle);
                if (!circleBounds.Contains(tapPoint))
                {
                    absoluteLayout.Children.Remove(_circle);
                    _circle = null;
                }
            }
        }

        private void CreateCircle(Point tapPoint)
        {
            var circleSize = 30;
            double offsetX = 15;
            double offsetY = -15;

            var circle = new Frame
            {
                WidthRequest = circleSize,
                HeightRequest = circleSize,
                CornerRadius = circleSize / 2,
                BackgroundColor = Colors.White,
                HasShadow = true,
                Padding = 0,
                Content = new Image
                {
                    Source = "pencil.svg",
                    Aspect = Aspect.AspectFit,
                    Margin = 3
                },
                AutomationId = "tapCircle"
            };

            var circleTap = new TapGestureRecognizer();
            circleTap.Tapped += OnCircleTapped;
            circle.GestureRecognizers.Add(circleTap);

            AbsoluteLayout.SetLayoutBounds(circle, new Rect(tapPoint.X - circleSize / 2 + offsetX,
                                                              tapPoint.Y - circleSize / 2 + offsetY,
                                                              circleSize,
                                                              circleSize));
            AbsoluteLayout.SetLayoutFlags(circle, AbsoluteLayoutFlags.None);

            absoluteLayout.Children.Add(circle);
            _circle = circle;
        }

        private async void OnCircleTapped(object sender, EventArgs e)
        {
            if (slidingPanel.IsVisible) return;

            if (_circle != null)
            {
                absoluteLayout.Children.Remove(_circle);
                _circle = null;
            }

            dimOverlay.IsVisible = true;
            dimOverlay.Opacity = 0;
            await dimOverlay.FadeTo(0.5, 250, Easing.CubicInOut);

            slidingPanel.IsVisible = true;
            await slidingPanel.TranslateTo(0, 0, 250, Easing.CubicInOut);
        }

        private async void OnDimOverlayTapped(object sender, EventArgs e)
        {
            if (!slidingPanel.IsVisible) return;

            await slidingPanel.TranslateTo(0, 500, 250, Easing.CubicInOut);
            slidingPanel.IsVisible = false;

            await dimOverlay.FadeTo(0, 250, Easing.CubicInOut);
            dimOverlay.IsVisible = false;
        }

        private void StartEditing()
        {
            _isEditingStarted = true;

            if (_circle != null)
            {
                absoluteLayout.Children.Remove(_circle);
                _circle = null;
            }
        }

        private void StopEditing()
        {
            _isEditingStarted = false;
        }
    }
}
