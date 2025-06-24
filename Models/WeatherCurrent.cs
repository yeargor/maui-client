using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public partial class WeatherCurrent : ObservableObject
    {
        [ObservableProperty]
        private double temperature_2m;

        [ObservableProperty]
        private double relative_humidity_2m;
    }
}
