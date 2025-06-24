namespace MauiDemo2;

using System.Reflection;
using Android.Content.Res;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MauiDemo2.Platforms.Android;
using Microsoft.Maui.Maps;

class MapCallbackHandler(CustomMapHandler mapHandler) : Java.Lang.Object, IOnMapReadyCallback
{
    public void OnMapReady(GoogleMap googleMap)
    {
        mapHandler.UpdateValue(nameof(IMap.Pins));
        mapHandler.Map?.SetOnMarkerClickListener(new CustomMarkerClickListener(mapHandler));
        mapHandler.Map?.SetOnInfoWindowClickListener(new CustomInfoWindowClickListener(mapHandler));

        googleMap.UiSettings.ZoomControlsEnabled = true;
        googleMap.UiSettings.MyLocationButtonEnabled = true;

        int screenHeight = Resources.System.DisplayMetrics.HeightPixels;
        int zoomControlHeight = 200;
        int verticalPadding = (screenHeight - zoomControlHeight) / 2;
        googleMap.SetPadding(0, 0, 20, verticalPadding);

        googleMap.UiSettings.MapToolbarEnabled = false;

        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("MauiDemo2.Resources.retro_theme.json");
        string json = string.Empty;
        using (var reader = new StreamReader(stream!))
        {
            json = reader.ReadToEnd();
        }
        googleMap.SetMapStyle(new Android.Gms.Maps.Model.MapStyleOptions(json));
    }
}