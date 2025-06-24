
using MauiDemo2.Platforms.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2
{
    public class Geolocator
    {
        public static IGeolocator Default = new GeolocatorImplementation();
    }
}
