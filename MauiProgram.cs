using MauiDemo2.Dtos;
using MauiDemo2.Services;
using MauiDemo2.ViewModel;
using MauiDemo2.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MauiDemo2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-Bold.ttf", "InterBold");
                });

            builder.Services.AddTransient<CardPage>();
            builder.Services.AddTransient<CardViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ProfilePageViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
