using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Models;
using MauiDemo2.ViewModels;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using Color = Android.Graphics.Color;
using IMap = Microsoft.Maui.Maps.IMap;
using Paint = Android.Graphics.Paint;

namespace MauiDemo2.Platforms.Android
{
    public class CustomMapHandler : MapHandler
    {
        public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
        new PropertyMapper<IMap, IMapHandler>(Mapper)
        {
            [nameof(IMap.Pins)] = MapPins
        };

        public CustomMapHandler() : base(CustomMapper, CommandMapper)
        {
            WeakReferenceMessenger.Default.Register<LocationPin>(this, (sender, updatedPin) =>
            {
                UpdateMarkerIcon(updatedPin);
            });
        }

        public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
            : base(mapper ?? CustomMapper, commandMapper ?? CommandMapper)
        {
            WeakReferenceMessenger.Default.Register<LocationPin>(this, (sender, updatedPin) =>
            {
                UpdateMarkerIcon(updatedPin);
            });
        }

        public List<(IMapPin pin, Marker marker)> Markers { get; } = new();

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);
            var mapReady = new MapCallbackHandler(this);
            PlatformView.GetMapAsync(mapReady);
        }

        private static new void MapPins(IMapHandler handler, IMap map)
        {
            if (handler is CustomMapHandler mapHandler)
            {
                var pinsToAdd = map.Pins.Where(x => x.MarkerId == null).ToList();
                var pinsToRemove = mapHandler.Markers.Where(x => !map.Pins.Contains(x.pin)).ToList();
                foreach (var marker in pinsToRemove)
                {
                    marker.marker.Remove();
                    mapHandler.Markers.Remove(marker);
                }

                mapHandler.AddPins(pinsToAdd);
            }
        }

        public void UpdateMarkerIcon(LocationPin updatedPin)
        {
            var markerToUpdate = Markers.FirstOrDefault(m =>
                m.pin.Location.Latitude == updatedPin.Location.Latitude &&
                m.pin.Location.Longitude == updatedPin.Location.Longitude);

            if (markerToUpdate.marker != null)
            {
                updatedPin.ImageSource.LoadImage(MauiContext, result =>
                {
                    if (result?.Value is BitmapDrawable { Bitmap: not null } bitmapDrawable)
                    {
                        var bitmap = GetMaximumBitmap(bitmapDrawable.Bitmap, 60, 60);
                        markerToUpdate.marker.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
                    }
                });
            }
        }

        public void SubscribeToCheckpointUpdates(MapRouteViewModel viewModel)
        {
            viewModel.CheckpointUpdated += UpdateMarkerIcon;
        }

        public void AddPins(IEnumerable<IMapPin> mapPins)
        {
            if (Map is null || MauiContext is null)
            {
                return;
            }

            foreach (var pin in mapPins)
            {
                var pinHandler = pin.ToHandler(MauiContext);
                if (pinHandler is IMapPinHandler mapPinHandler)
                {
                    var markerOption = mapPinHandler.PlatformView;
                    if (pin is CustomPin cp)
                    {
                        cp.ImageSource.LoadImage(MauiContext, result =>
                        {
                            if (result?.Value is BitmapDrawable { Bitmap: not null } bitmapDrawable)
                            {
                                var bitmap = GetMaximumBitmap(bitmapDrawable.Bitmap, 60, 60);

                                var finalBitmap = DrawTextOnBitmap(bitmap, cp.OrderOfVisit);
                                markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(finalBitmap));
                            }

                            AddMarker(Map, pin, markerOption);
                        });
                    }
                    else
                    {
                        AddMarker(Map, pin, markerOption);
                    }
                }
            }
        }
        private static Bitmap DrawTextOnBitmap(Bitmap bitmap, int? orderOfVisit)
        {
            if (orderOfVisit == null)
                return bitmap;

            var resultBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);
            var canvas = new Canvas(resultBitmap);

            var textPaint = new Paint
            {
                Color = Color.White,
                TextSize = resultBitmap.Width * 0.6f,
                TextAlign = Paint.Align.Center,
                AntiAlias = true,
                FakeBoldText = true
            };

            float centerX = resultBitmap.Width / 2f;
            float centerY = resultBitmap.Height / 2f;
            float textY = centerY - ((textPaint.Descent() + textPaint.Ascent()) / 2);

            canvas.DrawText(orderOfVisit.ToString(), centerX, textY, textPaint);

            return resultBitmap;
        }

        private void AddMarker(GoogleMap map, IMapPin pin, MarkerOptions markerOption)
        {
            markerOption.Anchor(0.5f, 0.5f);

            var marker = map.AddMarker(markerOption);
            pin.MarkerId = marker.Id;
            Markers.Add((pin, marker));
        }

        private static Bitmap GetMaximumBitmap(in Bitmap sourceImage, in float maxWidth, in float maxHeight)
        {
            var sourceSize = new Size(sourceImage.Width, sourceImage.Height);
            var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

            var width = Math.Max(maxResizeFactor * sourceSize.Width, 1);
            var height = Math.Max(maxResizeFactor * sourceSize.Height, 1);
            return Bitmap.CreateScaledBitmap(sourceImage, (int)width, (int)height, false)
                    ?? throw new InvalidOperationException("Failed to create Bitmap");
        }
    }
}
