using AutoMapper;
using CommunityToolkit.Maui;
using MauiDemo2.Dtos;
using MauiDemo2.Mappers;
using MauiDemo2.Services;
using MauiDemo2.ViewModel;
using MauiDemo2.ViewModels;
using MauiDemo2.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Extensions.DependencyInjection;
using MauiDemo2.Platforms.Android;
using AutoFixture;
using Sharpnado.Tabs;
using Sharpnado.CollectionView;

namespace MauiDemo2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiMaps()
                .UseSharpnadoTabs(loggerEnable: false)
                .UseSharpnadoCollectionView(loggerEnable: false)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-Bold.ttf", "InterBold");
                });
            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
            });
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddSingleton<RouteService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<SubscriptionService>();
            builder.Services.AddSingleton<MarkService>();
             builder.Services.AddSingleton<PointsAnotherService>();
            builder.Services.AddSingleton<PointsService>();
            builder.Services.AddSingleton<CurrentUserService>();
            builder.Services.AddSingleton<LocationSyncService>();
            builder.Services.AddSingleton<WeatherService>();
            builder.Services.AddTransient<CardPage>();
            builder.Services.AddTransient<CardViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RouteDetailsViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<CreatingRouteViewModel>();
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<FollowingPage>();
            builder.Services.AddTransient<MapRouteViewModel>();
             builder.Services.AddTransient<RouteDetailsPage>();
            builder.Services.AddSingleton<IFixture, Fixture>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddSingleton<AuthService>();

#if DEBUGk9
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
