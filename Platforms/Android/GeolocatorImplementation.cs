using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Location = Android.Locations.Location;
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;

namespace MauiDemo2.Platforms.Android
{
    public class GeolocatorImplementation : IGeolocator
    {
        GeolocationContinuousListener? locator;

        public async Task StartListening(IProgress<Microsoft.Maui.Devices.Sensors.Location> positionChangedProgress, CancellationToken cancellationToken)
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (permission != PermissionStatus.Granted)
                {
                    await Toast.Make("No permission").Show(CancellationToken.None);
                    return;
                }
            }

            locator = new GeolocationContinuousListener();
            var taskCompletionSource = new TaskCompletionSource();
            cancellationToken.Register(() =>
            {
                locator.Dispose();
                locator = null;
                taskCompletionSource.TrySetResult();
            });
            locator.OnLocationChangedAction = location =>
                positionChangedProgress.Report(
                    new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude));
            await taskCompletionSource.Task;
        }
    }

    internal class GeolocationContinuousListener : Java.Lang.Object, ILocationListener
    {
        public Action<Location>? OnLocationChangedAction { get; set; }

        LocationManager? locationManager;

        public GeolocationContinuousListener()
        {
            locationManager = (LocationManager?)global::Android.App.Application.Context.GetSystemService(global::Android.Content.Context.LocationService);
            locationManager?.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 100, this);
        }

        public void OnLocationChanged(Location location)
        {
            OnLocationChangedAction?.Invoke(location);
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            locationManager?.RemoveUpdates(this);
            locationManager?.Dispose();
        }
    }
}
