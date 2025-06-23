using System.Collections.ObjectModel;
using System.Windows.Input;
using AutoFixture;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Messages;
using MauiDemo2.Models;
using MauiDemo2.Models.Route;
using MauiDemo2.Services;
using MauiDemo2.Views;
using Microsoft.Maui.Controls.Maps;
using Point = MauiDemo2.Models.Point;

namespace MauiDemo2.ViewModels
{
    public partial class RouteDetailsViewModel: ObservableObject
    {
        private readonly IFixture _fixture;
        private readonly RouteService _routeService;
        private readonly PointsAnotherService _pointsService;
        
        [ObservableProperty]
        private RoutePostResponseDto? currentRoute;

        partial void OnCurrentRouteChanged(RoutePostResponseDto? value)
        {
            OnPropertyChanged(nameof(CurrentRoute));
        }
        
        [ObservableProperty]
        private double _totalDistance;
        
        [ObservableProperty]
        private double _totalTime;
        
        [ObservableProperty]
        private List<string> _images = new List<string> { "routepic1.svg", "routepic2.svg", "routepic1.svg", "routepic2.svg"};

        [ObservableProperty]
        private bool _isFullscreenImageVisible;

        [ObservableProperty]
        private bool _isFullscreenAddReviewVisible;

        [ObservableProperty]
        private string _selectedImage;

        [ObservableProperty]
        private float  _selectedRating = 5f;

        [ObservableProperty]
        private string _reviewText;

        [RelayCommand]
        private void Rate(int rating)
        {
            SelectedRating = (float)rating;
            OnPropertyChanged(nameof(SelectedRating));
        }

        [RelayCommand]
        private async Task SubmitReview()
        {
            if (string.IsNullOrWhiteSpace(ReviewText))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, введите текст отзыва", "OK");
                return;
            }
            var selectedStars = StarList.Count(s => s.StarColor == Brush.Gold);
            SelectedRating = selectedStars;
            var newReview = new Review
            {
                Username = "Wilson Press",
                UserImageUrl = "user2",
                ReviewText = ReviewText,
                Grade = SelectedRating
            };
            CurrentRoute?.Reviews.Add(newReview);
            ReviewText = string.Empty;
            SelectedRating = 0;
            ResetStarRatingBackgroundColor();
            IsFullscreenAddReviewVisible = false;
            await Shell.Current.DisplayAlert("Успех", "Ваш отзыв сохранен", "OK");
        }

        public ObservableCollection<LocationPin> LocationPins { get; } = new();
        public ObservableCollection<PlaceInfoResponseDto> PlacesForRoute { get; } = new();
        public ObservableCollection<AdditionalPlaceInfoResponseDto> AdditionalPlacesForRoute { get; } = new();

        public RouteDetailsViewModel(RouteService routeService, PointsAnotherService pointsService, IFixture fixture)
        {
            _fixture = fixture;
            _routeService = routeService;
            _pointsService = pointsService;
            SelectedRating = 0f; // Изначально все звезды пустые
            ResetStarRatingBackgroundColor();
        }

        [RelayCommand]
        private async Task NavigateToCardPage()
        {
            await Shell.Current.GoToAsync(nameof(CardPage));
        }

        [RelayCommand]
        private void ShowImageFullscreen(string image)
        {
            SelectedImage = image;
            IsFullscreenImageVisible = true;
        }

        [RelayCommand]
        private void CloseFullscreenImage()
        {
            IsFullscreenImageVisible = false;
        }

        [RelayCommand]
        private void ShowAddReviewFullscreen()
        {
            IsFullscreenAddReviewVisible = true;
        }

        [RelayCommand]
        private void CloseFullscreenAddReview()
        {
            IsFullscreenAddReviewVisible = false;
        }
        public async Task<RoutePostResponseDto> LoadRouteAsync(int routeId)
        {
            CurrentRoute = (await _routeService.GetMockRoutePostResponseDto()).FirstOrDefault();
            // Приводим Reviews к ObservableCollection, чтобы UI реагировал на изменения
            if (CurrentRoute != null && CurrentRoute.Reviews != null && !(CurrentRoute.Reviews is ObservableCollection<Review>))
            {
                CurrentRoute.Reviews = new ObservableCollection<Review>(CurrentRoute.Reviews);
            }
            PlacesForRoute.Clear();
            LocationPins.Clear();

            foreach (var place in CurrentRoute.PlacesInfos.OrderBy(p => p.OrderOfVisit))
            {
                PlacesForRoute.Add(place);

                var icon = place.IsCompleted
                    ? ImageSource.FromFile("route_point_completed.svg")
                    : ImageSource.FromFile("route_point.svg");

                var newLocation = _fixture.Build<LocationPin>()
                                           .With(x => x.ImageSource, icon)
                                           .With(x => x.Location, new Location(place.LocationInfo.Latitude, place.LocationInfo.Longitude))
                                           .With(x => x.Description, place.LocationInfo.Description)
                                           .With(x => x.OrderOfVisit, place.IsCompleted ? (int?)null : place.OrderOfVisit)
                                           .Create();
                LocationPins.Add(newLocation);
            }

            foreach (var addPlace in CurrentRoute.AdditionalPlacesInfos)
            {
                AdditionalPlacesForRoute.Add(addPlace);
                var newLocation = _fixture.Build<LocationPin>()
                                           .With(x => x.ImageSource, ImageSource.FromFile("route_point_additional.svg"))
                                           .With(x => x.Location, new Location(addPlace.LocationInfo.Latitude, addPlace.LocationInfo.Longitude))
                                           .With(x => x.Description, addPlace.LocationInfo.Description)
                                           .With(x => x.OrderOfVisit, (int?)null)
                                           .Create();
                LocationPins.Add(newLocation);
            }
            return CurrentRoute;
        }

        public async Task<(List<Location> Points, double Distance, double Time)> GetRoutePointsAsync(IEnumerable<PlaceInfoResponseDto> places)
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
                if (firstResult?["distance"] != null && firstResult?["time"] != null)
                {
                    double distance = (double)firstResult["distance"];
                    double time = (double)firstResult["time"];

                    var geometry = firstResult["geometry"]?[0];
                    if (geometry != null)
                    {
                        var routePoints = new List<Location>();
                        foreach (var point in geometry)
                        {
                            if (point?["lon"] != null && point?["lat"] != null)
                            {
                                double lon = (double)point["lon"];
                                double lat = (double)point["lat"];
                                routePoints.Add(new Location(lat, lon));
                            }
                        }
                        return (routePoints, distance, time);
                    }
                }

                return (new List<Location>(), 0, 0);
            }
            catch (Exception)
            {
                return (new List<Location>(), 0, 0);
            }
        }

        [RelayCommand]
        public async Task UpdateFavorite()
        {
            if (CurrentRoute != null && CurrentRoute.RouteInfo != null)
            {
                CurrentRoute.RouteInfo.UserLike.IsUserFavorite = !CurrentRoute.RouteInfo.UserLike.IsUserFavorite;
                OnPropertyChanged(nameof(CurrentRoute));
            }
        }

        public partial class StarRating : ObservableObject
        {
            public int Index { get; set; }
            private Brush _starColor;

            public Brush StarColor
            {
                get => _starColor;
                set => SetProperty(ref _starColor, value);
            }
        }

        [ObservableProperty]
        private ObservableCollection<StarRating> _starList = new();

        private void ResetStarRatingBackgroundColor()
        {
            StarList.Clear();
            for (int i = 1; i <= 5; i++)
            {
                StarList.Add(new StarRating { Index = i, StarColor = Brush.LightGray });
            }
        }

        [RelayCommand]
        private void StarRatingClicked(StarRating starRating)
        {
            ResetStarRatingBackgroundColor();
            for (int i = 0; i < starRating.Index; i++)
            {
                StarList[i].StarColor = Brush.Gold;
            }
        }
    }
}
