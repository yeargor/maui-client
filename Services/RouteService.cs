using MauiDemo2.Enums;
using MauiDemo2.Models;
using MauiDemo2.Models.Route;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Point = MauiDemo2.Models.Point;
using RouteCardResponseDto = MauiDemo2.Models.Route.RouteCardResponseDto;
using MauiDemo2.Models.Common;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Messages;

namespace MauiDemo2.Services
{
    public class RouteService
    {
        public event Action<int, UserLike?> RouteUpdated;
        private const string lorem = "Lorem ipsum dolor sit amet consectetur. Blandit id et at interdum in elit porta odio. A integer congue vitae id adipiscing elit sagittis curabitur. Orci aenean proin nulla dolor euismod molestie volutpat ут. Tincidunt adipiscing dictum faucibus ут ipsum.";
        private readonly UserService userService;
        private readonly CurrentUserService currentUserService;
        private readonly MarkService markService;
        private List<RouteFollowingMap> _mapPageRoutes = new List<RouteFollowingMap>();

        private readonly Dictionary<int, RouteFollowingMap> _routeStates = new();

        public RouteService(UserService userService,
                            MarkService markService,
                            CurrentUserService currentUserService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this.markService = markService ?? throw new ArgumentNullException(nameof(markService));

            _ = InitializeRoutesAsync();
        }

        private async Task InitializeRoutesAsync()
        {
            _mapPageRoutes = await GenerateTestMapPageRoutesAsync();
        }

        // Метод для CardPage (название сохранено)
        public async Task<List<RouteCardResponseDto>> GetRoutesAsync()
        {
            var token = string.Empty;
            try
            {
                token = Microsoft.Maui.Storage.Preferences.Get("jwtToken", string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения токена: {ex.Message}");
            }

            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("JWT токен не найден в Preferences");
            }

            var response = await client.GetAsync("http://10.0.2.2:5246/api/routes");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var routes = JsonSerializer.Deserialize<List<RouteCardResponseDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            System.Diagnostics.Debug.WriteLine($"Загружено маршрутов: {routes?.Count ?? 0}");
            if (routes != null)
            {
                foreach (var r in routes)
                {
                    System.Diagnostics.Debug.WriteLine($"Маршрут: {r.RouteInfo?.RouteTitle} (ID: {r.RouteInfo?.RouteId} UserCount: {r.UserCount})");
                }
            }
            return routes ?? new List<RouteCardResponseDto>();
        }

        // Метод для обновления маршрута (название сохранено)
        public async Task Update(int routeId)
        {
            var currentUser = currentUserService.GetCurrentUser();
            var route = await GetRouteById(routeId);
            var userLikeMark = route?.RouteInfo.UserLike;
            UserLike? newUserLike;

            if (userLikeMark == null)
            {
                // Создаём лайк
                await markService.CreateMarkAsync(new Mark {
                    UserId = currentUser.UserInfo.UserId,
                    RouteId = routeId,
                    MarkType = MarkType.Like
                });
                newUserLike = new UserLike { MarkId = 0, IsUserFavorite = true }; // MarkId можно обновить после получения с сервера
            }
            else
            {
                // Удаляем лайк
                await markService.DeleteMarkAsync(userLikeMark.MarkId);
                newUserLike = null;
            }

            // RouteUpdated?.Invoke(routeId, newUserLike);
            WeakReferenceMessenger.Default.Send(new RouteUpdatedMessage(routeId, newUserLike));
        }

        // // Метод для получения маршрутов по типу отметки (название сохранено)
        // public async Task<List<RouteMainPage>> GetRoutesByMarkTypeAsync(MarkType markType)
        // {
        //     var currentUser = currentUserService.GetCurrentUser();
        //     var userMarks = await markService.GetMarksByUserIdAsync(currentUser.UserInfo.UserId);

        //     var filteredRouteIds = userMarks
        //         .Where(m => m.MarkType == markType)
        //         .Select(m => m.RouteId)
        //         .Distinct()
        //         .ToList();

        //     return _cardPageRoutes
        //         .Where(r => filteredRouteIds.Contains(r.Id))
        //         .ToList();
        // }

        // Новый метод для MapPage
        public async Task<RouteFollowingMap> GetRouteForMapPageAsync(int routeId)
        {
            var route = _mapPageRoutes.FirstOrDefault(r => r.Id == routeId);

            if (route == null)
            {
                throw new KeyNotFoundException($"Маршрут с ID {routeId} не найден.");
            }

            return route;
        }

        public void SaveRouteState(RouteFollowingMap route)
        {
            _routeStates[route.Id] = route;
        }

        public RouteFollowingMap? LoadRouteState(int routeId)
        {
            _routeStates.TryGetValue(routeId, out var route);
            return route;
        }

        public void UpdatePlaceStatus(int routeId, int placeId, bool isCompleted)
        {
            var route = _mapPageRoutes.FirstOrDefault(r => r.Id == routeId);
            if (route != null)
            {
                var place = route.Places.FirstOrDefault(p => p.Id == placeId);
                if (place != null)
                {
                    place.IsCompleted = isCompleted;
                    System.Diagnostics.Debug.WriteLine($"Updated Place: {place.Name}, IsCompleted: {place.IsCompleted}");
                }
            }
        }

        public async Task<List<RoutePostResponseDto>> GetMockRoutePostResponseDto()
        {
            // return new List<RoutePostResponseDto>
            // {
            //     new RoutePostResponseDto
            //     {
            //         RouteInfo = new RouteInfoResponseDto
            //         {
            //             RouteId = 1,
            //             UserId = 123,
            //             RouteTitle = "Исторический маршрут по Гродно",
            //             RouteDescription = "Познавательный маршрут по главным достопримечательностям города",
            //             RouteType = RouteType.Walking,
            //             CreatedAt = DateTime.Now.AddDays(-2),
            //             RouteDistance = 5.2f,
            //             RouteRating = 4.8f,
            //             IsFavorite = true
            //         },
            //         AuthorUsername = "traveler_grodno",
            //         PlacesInfos = new List<PlaceInfoResponseDto>
            //         {
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 1,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Старый замок",
            //                     Description = lorem,
            //                     Latitude = 53.6838,
            //                     Longitude = 23.8315,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 2,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Новый замок",
            //                     Description = lorem,
            //                     Latitude = 53.6844,
            //                     Longitude = 23.8352,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 3,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Парк Жилибера",
            //                     Description = lorem,
            //                     Latitude = 53.6793,
            //                     Longitude = 23.8319,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 4,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Дендропарк",
            //                     Description = lorem,
            //                     Latitude = 53.6735,
            //                     Longitude = 23.8258,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 5,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Озеро Юбилейное",
            //                     Description = lorem,
            //                     Latitude = 53.6687,
            //                     Longitude = 23.8132,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 6,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Коложская церковь",
            //                     Description = lorem,
            //                     Latitude = 53.6880,
            //                     Longitude = 23.8376,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             }
            //         },
            //         AdditionalPlacesInfos = new List<AdditionalPlaceInfoResponseDto>
            //         {
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Музей истории",
            //                     Description = lorem,
            //                     Latitude = 53.6850,
            //                     Longitude = 23.8300,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Театр драмы",
            //                     Description = lorem,
            //                     Latitude = 53.6820,
            //                     Longitude = 23.8280,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Гродненский зоопарк",
            //                     Description = lorem,
            //                     Latitude = 53.6800,
            //                     Longitude = 23.8260,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             }
            //         },
            //         RoutePathImageUrl = "white.jpg",
            //         RouteTime = 120, // В минутах
            //         Reviews = new ObservableCollection<Review>
            //         {
            //             new Review
            //             {
            //                 Id = 1,
            //                 UserId = 1,
            //                 RouteId = 1,
            //                 Username = "Wilson Press",
            //                 UserImageUrl = "user1.svg",
            //                 ReviewText = "Очень понравилось, рекомендую всем",
            //                 Grade = 5f 
            //             },
            //             new Review
            //             {
            //                 Id = 2,
            //                 UserId = 1,
            //                 RouteId = 1,
            //                 Username = "Wilson Press",
            //                 UserImageUrl = "user2.svg",
            //                 ReviewText = "Не понравилось, рекомендую не всем",
            //                 Grade = 3f
            //             }
            //         },
            //         RoutePath = new List<Point>
            //         {
            //             new Point { Id = 1, LocationId = 1, Latitude = 53.6838, Longitude = 23.8315 },
            //             new Point { Id = 2, LocationId = 2, Latitude = 53.6844, Longitude = 23.8352 },
            //             new Point { Id = 3, LocationId = 3, Latitude = 53.6880, Longitude = 23.8376 }
            //         }
            //     },
            //     new RoutePostResponseDto
            //     {
            //         RouteInfo = new RouteInfoResponseDto
            //         {
            //             RouteId = 2,
            //             UserId = 124,
            //             RouteTitle = "Культурный маршрут по Гродно",
            //             RouteDescription = "Маршрут по культурным и историческим местам города",
            //             RouteType = RouteType.Walking,
            //             CreatedAt = DateTime.Now.AddDays(-5),
            //             RouteDistance = 6.5f,
            //             RouteRating = 4.9f,
            //             IsFavorite = false
            //         },
            //         AuthorUsername = "culture_lover",
            //         PlacesInfos = new List<PlaceInfoResponseDto>
            //         {
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 1,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Филармония",
            //                     Description = lorem,
            //                     Latitude = 53.6900,
            //                     Longitude = 23.8500,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 2,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Краеведческий музей",
            //                     Description = lorem,
            //                     Latitude = 53.6920,
            //                     Longitude = 23.8520,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 3,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Площадь Ленина",
            //                     Description = lorem,
            //                     Latitude = 53.6940,
            //                     Longitude = 23.8540,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 4,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Дом-музей Элизы Ожешко",
            //                     Description = lorem,
            //                     Latitude = 53.6960,
            //                     Longitude = 23.8560,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 5,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Театр кукол",
            //                     Description = lorem,
            //                     Latitude = 53.6980,
            //                     Longitude = 23.8580,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 6,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Гродненская библиотека",
            //                     Description = lorem,
            //                     Latitude = 53.7000,
            //                     Longitude = 23.8600,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             }
            //         },
            //         AdditionalPlacesInfos = new List<AdditionalPlaceInfoResponseDto>
            //         {
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Кинотеатр Октябрь",
            //                     Description = lorem,
            //                     Latitude = 53.7020,
            //                     Longitude = 23.8620,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Галерея искусств",
            //                     Description = lorem,
            //                     Latitude = 53.7040,
            //                     Longitude = 23.8640,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Парк культуры",
            //                     Description = lorem,
            //                     Latitude = 53.7060,
            //                     Longitude = 23.8660,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             }
            //         },
            //         RoutePathImageUrl = "white.jpg",
            //         RouteTime = 150,
            //         Reviews = new ObservableCollection<Review>(),
            //         RoutePath = new List<Point>()
            //     },
            //     new RoutePostResponseDto
            //     {
            //         RouteInfo = new RouteInfoResponseDto
            //         {
            //             RouteId = 3,
            //             UserId = 125,
            //             RouteTitle = "Природный маршрут по Гродно",
            //             RouteDescription = "Маршрут по природным достопримечательностям города",
            //             RouteType = RouteType.Walking,
            //             CreatedAt = DateTime.Now.AddDays(-10),
            //             RouteDistance = 7.0f,
            //             RouteRating = 4.7f,
            //             IsFavorite = true
            //         },
            //         AuthorUsername = "nature_explorer",
            //         PlacesInfos = new List<PlaceInfoResponseDto>
            //         {
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 1,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Ботанический сад",
            //                     Description = lorem,
            //                     Latitude = 53.7100,
            //                     Longitude = 23.8700,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 2,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Озеро Белое",
            //                     Description = lorem,
            //                     Latitude = 53.7120,
            //                     Longitude = 23.8720,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 3,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Лесопарк",
            //                     Description = lorem,
            //                     Latitude = 53.7140,
            //                     Longitude = 23.8740,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 4,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Река Неман",
            //                     Description = lorem,
            //                     Latitude = 53.7160,
            //                     Longitude = 23.8760,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 5,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Парк Победы",
            //                     Description = lorem,
            //                     Latitude = 53.7180,
            //                     Longitude = 23.8780,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new PlaceInfoResponseDto
            //             {
            //                 OrderOfVisit = 6,
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Гродненский лес",
            //                     Description = lorem,
            //                     Latitude = 53.7200,
            //                     Longitude = 23.8800,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             }
            //         },
            //         AdditionalPlacesInfos = new List<AdditionalPlaceInfoResponseDto>
            //         {
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Озеро Юбилейное",
            //                     Description = lorem,
            //                     Latitude = 53.7220,
            //                     Longitude = 23.8820,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Парк Жилибера",
            //                     Description = lorem,
            //                     Latitude = 53.7240,
            //                     Longitude = 23.8840,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             },
            //             new AdditionalPlaceInfoResponseDto
            //             {
            //                 LocationInfo = new LocationInfoResponseDto
            //                 {
            //                     LocationName = "Дендропарк",
            //                     Description = lorem,
            //                     Latitude = 53.7260,
            //                     Longitude = 23.8860,
            //                     LocationImageUrl = "white.jpg"
            //                 }
            //             }
            //         },
            //         RoutePathImageUrl = "white.jpg",
            //         RouteTime = 180,
            //         Reviews = new ObservableCollection<Review>(),
            //         RoutePath = new List<Point>()
            //     }
            // };
            return null;
        }

        // Получение маршрутов для ProfilePage по id пользователя
        public async Task<List<RouteCardUserResponseDto>> GetRoutesForProfilePage()
        {
            var token = JwtService.GetJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine("JWT токен не найден в Preferences");
                return new List<RouteCardUserResponseDto>();
            }

            var currentUser = currentUserService.GetCurrentUser();
            int userId = currentUser.UserInfo.UserId;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"http://10.0.2.2:5246/api/routes/user/{userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var routes = JsonSerializer.Deserialize<List<RouteCardUserResponseDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return routes ?? new List<RouteCardUserResponseDto>();
        }

        private async Task<List<RouteMainPage>> GenerateTestCardPageRoutesAsync()
        {
            List<User> users = await userService.GetUsersAsync();

            if (users == null || users.Count == 0)
            {
                throw new InvalidOperationException("Users list is empty or null.");
            }

            return new List<RouteMainPage>
            {
                new RouteMainPage
                {
                    Id = 1,
                    Name = "Горный маршрут",
                    Description = lorem,
                    TimesCompleted = 42,
                    Rating = 4.8f,
                    Distance = 12.5,
                    DaysAgo = "2 дня назад",
                    User = users.ElementAtOrDefault(0)
                },
                new RouteMainPage
                {
                    Id = 2,
                    Name = "Лесная тропа",
                    Description = lorem,
                    TimesCompleted = 28,
                    Rating = 4.5f,
                    Distance = 8.2,
                    DaysAgo = "5 дней назад",
                    User = users.ElementAtOrDefault(1)
                },
                new RouteMainPage
                {
                    Id = 3,
                    Name = "Озерный круг",
                    Description = lorem,
                    TimesCompleted = 35,
                    Rating = 4.7f,
                    Distance = 10.0,
                    DaysAgo = "1 день назад",
                    User = users.ElementAtOrDefault(2)
                }
            };
        }

        public async Task<List<RouteFollowingMap>> GenerateTestMapPageRoutesAsync()
        {
            return new List<RouteFollowingMap>
            {
                CreateRouteFollowingMap(1, "Исторический маршрут по Гродно", GeneratePlaces(), GenerateAdditionalPlaces()),
                CreateRouteFollowingMap(2, "Природный маршрут по Гродно", GenerateNaturePlaces(), GenerateNatureAdditionalPlaces()),
                CreateRouteFollowingMap(3, "Культурный маршрут по Гродно", GenerateCulturalPlaces(), GenerateCulturalAdditionalPlaces())
            };
        }

        private RouteFollowingMap CreateRouteFollowingMap(int id, string name, List<Place> places, List<AdditionalPlace> additionalPlaces)
        {
            return new RouteFollowingMap
            {
                Id = id,
                Name = name,
                Places = places,
                AdditionalPlaces = additionalPlaces
            };
        }


        private List<Place> GeneratePlaces()
        {
            return new List<Place>
            {
                new Place { Id = 1, Name = "Старый замок", Description = "Древний замок XV века", Point = new Point { Id = 1, Latitude = 53.6838, Longitude = 23.8315 }, OrderOfVisit = 1 },
                new Place { Id = 2, Name = "Новый замок", Description = "Резиденция польских королей", Point = new Point { Id = 2, Latitude = 53.6844, Longitude = 23.8352 }, OrderOfVisit = 2 },
                new Place { Id = 3, Name = "Парк Жилибера", Description = "Живописный парк", Point = new Point { Id = 3, Latitude = 53.6793, Longitude = 23.8319 }, OrderOfVisit = 3 },
                new Place { Id = 4, Name = "Дендропарк", Description = "Природный заповедник", Point = new Point { Id = 4, Latitude = 53.6735, Longitude = 23.8258 }, OrderOfVisit = 4 },
                new Place { Id = 5, Name = "Филармония", Description = "Культурное учреждение", Point = new Point { Id = 5, Latitude = 53.6900, Longitude = 23.8500 }, OrderOfVisit = 5 },
                new Place { Id = 6, Name = "Краеведческий музей", Description = "Исторический музей", Point = new Point { Id = 6, Latitude = 53.6920, Longitude = 23.8520 }, OrderOfVisit = 6 },
                new Place { Id = 7, Name = "Площадь Ленина", Description = "Главная площадь", Point = new Point { Id = 7, Latitude = 53.6940, Longitude = 23.8540 }, OrderOfVisit = 7 },
                new Place { Id = 8, Name = "Дом-музей Элизы Ожешко", Description = "Музей писательницы", Point = new Point { Id = 8, Latitude = 53.6960, Longitude = 23.8560 }, OrderOfVisit = 8 }
            };
        }

        private List<AdditionalPlace> GenerateAdditionalPlaces()
        {
            return new List<AdditionalPlace>
            {
                new AdditionalPlace { Id = 1, Name = "Коложская церковь", Description = "Древняя православная церковь XII века", Point = new Point { Id = 9, Latitude = 53.6880, Longitude = 23.8376 }, Type = AdditionalPlaceType.Monument },
                new AdditionalPlace { Id = 2, Name = "Кинотеатр Октябрь", Description = "Современный кинотеатр", Point = new Point { Id = 10, Latitude = 53.7020, Longitude = 23.8620 }, Type = AdditionalPlaceType.Galery },
                new AdditionalPlace { Id = 3, Name = "Галерея искусств", Description = "Выставочный центр", Point = new Point { Id = 11, Latitude = 53.7040, Longitude = 23.8640 }, Type = AdditionalPlaceType.Monument },
                new AdditionalPlace { Id = 4, Name = "Парк культуры", Description = "Место для отдыха", Point = new Point { Id = 12, Latitude = 53.7060, Longitude = 23.8660 }, Type = AdditionalPlaceType.Galery },
                new AdditionalPlace { Id = 5, Name = "Озеро Юбилейное", Description = "Спокойное место для прогулок", Point = new Point { Id = 13, Latitude = 53.6687, Longitude = 23.8132 }, Type = AdditionalPlaceType.Galery }
            };
        }

        private List<Place> GenerateNaturePlaces()
        {
            // return new List<Place>
            // {
            //     new Place { Id = 9, Name = "Ботанический сад", Description = "Сад с редкими растениями", Point = new Point { Latitude = 53.7100, Longitude = 23.8700 }, OrderOfVisit = 1 },
            //     new Place { Id = 10, Name = "Озеро Белое", Description = "Живописное озеро", Point = new Point { Latitude = 53.7120, Longitude = 23.8720 }, OrderOfVisit = 2 },
            //     new Place { Id = 11, Name = "Лесопарк", Description = "Место для прогулок и пикников", Point = new Point { Latitude = 53.7140, Longitude = 23.8740 }, OrderOfVisit = 3 },
            //     new Place { Id = 12, Name = "Река Неман", Description = lorem, Point= Latitude = 53.7160,
            //     Longitude = 23.8760,
            //     OrderOfVisit = 4 },
            //     new Place { Id = 13, Name = "Парк Победы", Description = "Зеленая зона с мемориалами", Point = new Point { Latitude = 53.7180, Longitude = 23.8780 }, OrderOfVisit = 5 },
            //     new Place { Id = 14, Name = "Гродненский лес", Description = "Обширный лесной массив", Point = new Point { Latitude = 53.7200, Longitude = 23.8800 }, OrderOfVisit = 6 },
            //     new Place { Id = 15, Name = "Озеро Юбилейное", Description = "Тихое место для отдыха", Point = new Point { Latitude = 53.7220, Longitude = 23.8820 }, OrderOfVisit = 7 },
            //     new Place { Id = 16, Name = "Дендропарк", Description = "Коллекция редких деревьев", Point = new Point { Latitude = 53.7240, Longitude = 23.8840 }, OrderOfVisit = 8 }
            // };
            return null;
        }

        private List<AdditionalPlace> GenerateNatureAdditionalPlaces()
        {
            return new List<AdditionalPlace>
            {
                new AdditionalPlace { Name = "Парк Жилибера", Description = "Популярное место для отдыха", Point = new Point { Latitude = 53.7260, Longitude = 23.8860 }, Type = AdditionalPlaceType.Monument },
                new AdditionalPlace { Name = "Заповедник Беловежская пуща", Description = "Национальный парк", Point = new Point { Latitude = 53.7280, Longitude = 23.8880 }, Type = AdditionalPlaceType.Museum },
                new AdditionalPlace { Name = "Грот Смока", Description = "Природная достопримечательность", Point = new Point { Latitude = 53.7300, Longitude = 23.8900 }, Type = AdditionalPlaceType.Galery }
            };
        }
        private List<Place> GenerateCulturalPlaces()
        {
            return new List<Place>
            {
                new Place { Id = 17, Name = "Филармония", Description = "Концертный зал", Point = new Point { Latitude = 53.6900, Longitude = 23.8500 }, OrderOfVisit = 1 },
                new Place { Id = 18, Name = "Краеведческий музей", Description = "Музей о культуре Гродно", Point = new Point { Latitude = 53.6920, Longitude = 23.8520 }, OrderOfVisit = 2 },
                new Place { Id = 19, Name = "Дом-музей Элизы Ожешко", Description = "Место жизни известной писательницы", Point = new Point { Latitude = 53.6960, Longitude = 23.8560 }, OrderOfVisit = 3 },
                new Place { Id = 20, Name = "Гродненская библиотека", Description = "Библиотека с редкими книгами", Point = new Point { Latitude = 53.7000, Longitude = 23.8600 }, OrderOfVisit = 4 }
            };
        }

        private List<AdditionalPlace> GenerateCulturalAdditionalPlaces()
        {
            return new List<AdditionalPlace>
            {
                new AdditionalPlace { Name = "Кинотеатр Октябрь", Description = "Популярный кинотеатр", Point = new Point { Latitude = 53.7020, Longitude = 23.8620 }, Type = AdditionalPlaceType.Galery },
                new AdditionalPlace { Name = "Галерея искусств", Description = "Выставочный центр", Point = new Point { Latitude = 53.7040, Longitude = 23.8640 }, Type = AdditionalPlaceType.Monument }
            };
        }

        public async Task<RouteCardResponseDto?> GetRouteById(int routeId)
        {
            var token = JwtService.GetJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine("JWT токен не найден в Preferences");
                return null;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"http://10.0.2.2:5246/api/routes/{routeId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[RouteService] JSON response for GetRouteById: {json}");
            return JsonSerializer.Deserialize<RouteCardResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}