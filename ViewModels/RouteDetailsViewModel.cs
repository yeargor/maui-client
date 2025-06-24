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
        private readonly ReviewsService _reviewsService;
        private readonly CurrentUserService _currentUserService;
        
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
        private string _selectedImage = string.Empty;

        [ObservableProperty]
        private string _reviewText = string.Empty;

        [ObservableProperty]
        private float _selectedRating = 5f;

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
            var selectedStars = _starList.Count(s => s.StarColor == Brush.Gold);
            SelectedRating = selectedStars;

            var user = _currentUserService.GetCurrentUser();
            if (user == null || CurrentRoute == null || CurrentRoute.RouteInfo == null)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не удалось получить пользователя или маршрут", "OK");
                return;
            }

            var dto = new CreateReviewRequestDto
            {
                UserId = user.UserInfo.UserId,
                RouteId = CurrentRoute.RouteInfo.RouteId,
                Text = ReviewText,
                Grade = SelectedRating
            };

            var success = await _reviewsService.CreateReviewAsync(dto);
            if (success)
            {
                var newReview = new Review
                {
                    UserId = user.UserInfo.UserId,
                    RouteId = CurrentRoute.RouteInfo.RouteId,
                    Username = user.UserInfo.Username,
                    UserImageUrl = user.UserInfo.ImageUrl ?? string.Empty,
                    ReviewText = ReviewText,
                    Grade = SelectedRating
                };
                CurrentRoute.Reviews.Add(newReview);
                ReviewText = string.Empty;
                SelectedRating = 0;
                ResetStarRatingBackgroundColor();
                IsFullscreenAddReviewVisible = false;
                await Shell.Current.DisplayAlert("Успех", "Ваш отзыв сохранен", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не удалось отправить отзыв", "OK");
            }
        }

        public ObservableCollection<LocationPin> LocationPins { get; } = new();
        public ObservableCollection<PlaceInfoResponseDto> PlacesForRoute { get; } = new();
        public ObservableCollection<AdditionalPlaceInfoResponseDto> AdditionalPlacesForRoute { get; } = new();

        public RouteDetailsViewModel(RouteService routeService, PointsAnotherService pointsService, IFixture fixture, ReviewsService reviewsService, CurrentUserService currentUserService)
        {
            _fixture = fixture;
            _routeService = routeService;
            _pointsService = pointsService;
            _reviewsService = reviewsService;
            _currentUserService = currentUserService;
            SelectedRating = 0f;
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
            CurrentRoute = await _routeService.GetRouteById(routeId);
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
                                        //    .With(x => x.Description, place.LocationInfo.Description)
                                        //    .With(x => x.OrderOfVisit, place.IsCompleted ? (int?)null : place.OrderOfVisit)
                                           .Create();
                LocationPins.Add(newLocation);
            }

            foreach (var addPlace in CurrentRoute.AdditionalPlacesInfos)
            {
                AdditionalPlacesForRoute.Add(addPlace);
                var newLocation = _fixture.Build<LocationPin>()
                                           .With(x => x.ImageSource, ImageSource.FromFile("route_point_additional.svg"))
                                           .With(x => x.Location, new Location(addPlace.LocationInfo.Latitude, addPlace.LocationInfo.Longitude))
                                        //    .With(x => x.Description, addPlace.LocationInfo.Description)
                                           .With(x => x.OrderOfVisit, (int?)null)
                                           .Create();
                LocationPins.Add(newLocation);
            }
            return CurrentRoute;
        }

        public Task<(List<Location> Points, double Distance, double Time)> GetRoutePointsAsync(IEnumerable<PlaceInfoResponseDto> places)
        {
            if (CurrentRoute == null)
                return Task.FromResult((new List<Location>(), 0d, 0d));

            var points = CurrentRoute.RoutePath?
                .Select(c => new Location(c.Latitude, c.Longitude))
                .ToList() ?? new List<Location>();

            double distance = CurrentRoute.RouteInfo?.RouteDistance ?? 0;
            double time = CurrentRoute.RouteDuration;

            return Task.FromResult((points, distance, time));
        }

         [RelayCommand]
        public async Task UpdateFavorite(RouteCardUserResponseDto route)
        {
            if (route != null)
            {
                await _routeService.Update(route.RouteInfo.RouteId);
                OnPropertyChanged(nameof(CurrentRoute));
            }
        }

        public partial class StarRating : ObservableObject
        {
            public int Index { get; set; }
            private Brush _starColor = Brush.LightGray;

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
            _starList.Clear();
            for (int i = 1; i <= 5; i++)
            {
                _starList.Add(new StarRating { Index = i, StarColor = Brush.LightGray });
            }
        }

        [RelayCommand]
        private void StarRatingClicked(StarRating starRating)
        {
            ResetStarRatingBackgroundColor();
            for (int i = 0; i < starRating.Index; i++)
            {
                _starList[i].StarColor = Brush.Gold;
            }
        }
    }
}
