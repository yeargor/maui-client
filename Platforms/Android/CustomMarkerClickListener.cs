using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MauiDemo2.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Gms.Maps.GoogleMap;
using MauiDemo2.Models;

namespace MauiDemo2.Platforms.Android
{
    internal class CustomMarkerClickListener : Java.Lang.Object, IOnMarkerClickListener
    {
        private readonly CustomMapHandler mapHandler;

        public CustomMarkerClickListener(CustomMapHandler mapHandler)
        {
            this.mapHandler = mapHandler;   
        }

      public bool OnMarkerClick(Marker marker)
        {
            var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);

            if (pin.marker != null && pin.pin != null)
            {
                System.Diagnostics.Debug.WriteLine($"Pin clicked: {pin.pin.Label}, Coordinates: {pin.pin.Location.Latitude}, {pin.pin.Location.Longitude}");

                WeakReferenceMessenger.Default.Send(new PinClickedMessage(pin.pin.Label, pin.pin.Location));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Pin or marker is null. Unable to send PinClickedMessage.");
            }

            return true;
        }
    }
}
