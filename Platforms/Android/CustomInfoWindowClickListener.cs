using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Gms.Maps.GoogleMap;

namespace MauiDemo2.Platforms.Android
{
    internal class CustomInfoWindowClickListener(CustomMapHandler mapHandler)
    : Java.Lang.Object, IOnInfoWindowClickListener
    {
        public void OnInfoWindowClick(Marker marker)
        {
            var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
            pin.pin?.SendInfoWindowClick();
        }
    }
}
