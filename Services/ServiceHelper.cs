using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Services
{
    public static class ServiceHelper
    {
        public static T GetService<T>() => Current.GetService<T>();

        public static IServiceProvider Current =>
#if ANDROID
            MauiApplication.Current.Services;
#else
            App.Current.Services;
#endif
    }
}
