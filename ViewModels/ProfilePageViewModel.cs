using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Enums;
using MauiDemo2.Messages;
using MauiDemo2.Models;
using MauiDemo2.Models.Common;
using MauiDemo2.Models.Route;
using MauiDemo2.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiDemo2.ViewModel
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        public ObservableCollection<RouteCardUserResponseDto> Routes { get; set; } = new ObservableCollection<RouteCardUserResponseDto>();
        public ObservableCollection<RouteCardUserResponseDto> FavoriteRoutes { get; set; } = new();
        public ObservableCollection<RouteCardUserResponseDto> MyRoutes { get; set; } = new();
        public ObservableCollection<RouteCardUserResponseDto> DoneRoutes { get; set; } = new();

        [ObservableProperty]
        private string activeTab = "map";

        [ObservableProperty]
        private int followersCount;

        [ObservableProperty]
        private int followingCount;

        [ObservableProperty]
        private int likeMarksCount;
        [ObservableProperty]
        private int saveMarksCount;
        [ObservableProperty]
        private int doneMarksCount;
        [ObservableProperty]
        private double totalDoneDistance;
        private readonly RouteService _routeService;
        private readonly MarkService _markService;
        private readonly SubscriptionService _subscriptionService;
        private readonly CurrentUserService _currentUserService;

        private UserProfileResponseDto _userProfile;
        public UserProfileResponseDto UserProfile
        {
            get => _userProfile;
            set => SetProperty(ref _userProfile, value);
        }

        private ObservableCollection<RouteCardUserResponseDto> _currentRoutes = new();
        public ObservableCollection<RouteCardUserResponseDto> CurrentRoutes
        {
            get => _currentRoutes;
            set => SetProperty(ref _currentRoutes, value);
        }

        public ProfilePageViewModel(RouteService routeService, SubscriptionService subscriptionService, CurrentUserService currentUserService, MarkService markService)
        {
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _markService = markService ?? throw new ArgumentNullException(nameof(markService));
            // _routeService.RouteUpdated += OnRouteUpdated;
            WeakReferenceMessenger.Default.Register<RouteUpdatedMessage>(this, (r, msg) =>
            {
                var (routeId, userLike) = msg.Value;
                OnRouteUpdated(routeId, userLike);
            });
            _ = LoadRoutesAsync();
        }

        private async Task GetUserProfile()
        {
            var currentUser = _currentUserService.GetCurrentUser();
            var token = JwtService.GetJwtToken();
            var userService = ServiceHelper.GetService<UserService>();
            UserProfile = await userService.GetUserProfileByIdAsync(currentUser.UserInfo.UserId, token);
            System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] CurrentUser set: {{ UserId: {UserProfile?.UserInfo?.UserId}, Username: {UserProfile?.UserInfo?.Username}, FirstName: {UserProfile?.UserInfo?.FirstName}, LastName: {UserProfile?.UserInfo?.LastName}, Followers: {UserProfile?.FollowersNumber}, Subscriptions: {UserProfile?.SubscriptionsNumber} }}");
        }

        private async Task LoadRoutesAsync()
        {
            await GetUserProfile();
            var routes = await _routeService.GetRoutesForProfilePage();
            Routes = new ObservableCollection<RouteCardUserResponseDto>(routes);

            System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] Routes count: {Routes.Count}");
            // Логируем UserProfile.UserInfo
            if (UserProfile?.UserInfo != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] UserInfo: Id={UserProfile.UserInfo.UserId}, Username={UserProfile.UserInfo.Username}, FirstName={UserProfile.UserInfo.FirstName}, LastName={UserProfile.UserInfo.LastName}, Image={UserProfile.UserInfo.ImageUrl}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] UserInfo is null");
            }

            // Обновляем буферные списки
            FavoriteRoutes = new ObservableCollection<RouteCardUserResponseDto>(
                Routes.Where(r => r.UserMarks != null && r.UserMarks.Any(m => m.MarkType == MarkType.Like))
            );

            MyRoutes = new ObservableCollection<RouteCardUserResponseDto>(
                Routes.Where(r => r.UserMarks != null && r.UserMarks.Any(m => m.MarkType == MarkType.Mine))
            );

            DoneRoutes = new ObservableCollection<RouteCardUserResponseDto>(
                Routes.Where(r => r.UserMarks != null && r.UserMarks.Any(m => m.MarkType == MarkType.Done))
            );

            LikeMarksCount = FavoriteRoutes.Count;
            SaveMarksCount = MyRoutes.Count;
            DoneMarksCount = DoneRoutes.Count;

            if (UserProfile != null)
            {
                FollowersCount = UserProfile.FollowersNumber;
                FollowingCount = UserProfile.SubscriptionsNumber;
            }

            TotalDoneDistance = Math.Round(Routes.Sum(r => r.RouteInfo.RouteDistance), 2);

            CurrentRoutes = MyRoutes;

            await UpdateSortedRoutes();
        }

        public Command<string> ChangeTabCommand => new Command<string>(async (tab) =>
        {
            ActiveTab = tab;
            switch (ActiveTab)
            {
                case "map":
                    CurrentRoutes = MyRoutes;
                    break;
                case "path_circle":
                    CurrentRoutes = DoneRoutes;
                    break;
                case "heart_black":
                    CurrentRoutes = FavoriteRoutes;
                    break;
                default:
                    CurrentRoutes = new ObservableCollection<RouteCardUserResponseDto>();
                    break;
            }
            await UpdateSortedRoutes();
        });

        [RelayCommand]
        public async void UpdateFavorite(RouteCardUserResponseDto route)
        {
            if (route != null)
            {
                System.Diagnostics.Debug.WriteLine($"[CardViewModel] UpdateFavorite: BEFORE call, RouteId={route.RouteInfo.RouteId}, UserLike={(route.RouteInfo.UserLike != null ? route.RouteInfo.UserLike.IsUserFavorite.ToString() : "null")}");
                await _routeService.Update(route.RouteInfo.RouteId);
                System.Diagnostics.Debug.WriteLine($"[CardViewModel] UpdateFavorite: AFTER call, RouteId={route.RouteInfo.RouteId}, UserLike={(route.RouteInfo.UserLike != null ? route.RouteInfo.UserLike.IsUserFavorite.ToString() : "null")}");
                Routes = new ObservableCollection<RouteCardUserResponseDto>(Routes);
            }
        }

        private async void OnRouteUpdated(int routeId, UserLike? userLike)
        {
            // Просто полностью обновляем маршруты и связанные коллекции
            await LoadRoutesAsync();
            System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] OnRouteUpdated: Reloaded all routes after update for RouteId={routeId}");
        }

        private async Task UpdateSortedRoutes()
        {
            FavoriteRoutes = new ObservableCollection<RouteCardUserResponseDto>(
                Routes.Where(r => r.UserMarks != null && r.UserMarks.Any(m => m.MarkType == MarkType.Like))
            );

            MyRoutes = new ObservableCollection<RouteCardUserResponseDto>(
                Routes.Where(r => r.UserMarks != null && r.UserMarks.Any(m => m.MarkType == MarkType.Mine))
            );

            DoneRoutes = new ObservableCollection<RouteCardUserResponseDto>(
                Routes.Where(r => r.UserMarks != null && r.UserMarks.Any(m => m.MarkType == MarkType.Done))
            );

            OnPropertyChanged(nameof(FavoriteRoutes));
            OnPropertyChanged(nameof(MyRoutes));
            OnPropertyChanged(nameof(DoneRoutes));
        }
    }
}
