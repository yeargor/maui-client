using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class DeviceLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DeviceLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
