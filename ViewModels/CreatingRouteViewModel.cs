using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiDemo2.Models;
using MauiDemo2.Services;
using MauiDemo2.Views;
using System;
using System.Windows.Input;

namespace MauiDemo2.ViewModels
{
    public partial class CreatingRouteViewModel : ObservableObject
    {
        private readonly LocationSyncService _locationSyncService;

        [ObservableProperty]
        private double latitude;

        [ObservableProperty]
        private double longitude;

        [ObservableProperty]
        private bool isListening;

        [ObservableProperty]
        private string? listeningButtonText;

        public CreatingRouteViewModel(LocationSyncService locationSyncService)
        {
            _locationSyncService = locationSyncService;

            WeakReferenceMessenger.Default.Register<DeviceLocation>(this, (sender, deviceLocation) =>
            {
                Latitude = deviceLocation.Latitude;
                Longitude = deviceLocation.Longitude;
            });

            ListeningButtonText = "Start";
        }

        [RelayCommand]
        private void ChangeListeningMode()
        {
            if (!IsListening)
            {
                _ = _locationSyncService.Start();
                IsListening = true;
                ListeningButtonText = "Stop";
            }
            else
            {
                _locationSyncService.Stop();
                IsListening = false;
                ListeningButtonText = "Start";
            }
        }
    }
}
