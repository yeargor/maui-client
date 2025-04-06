using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDemo2.Dtos;
using MauiDemo2.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiDemo2.ViewModel
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string message;

        [ObservableProperty]
        UserRequestDto user;

        public IAsyncRelayCommand GetUserCommand { get; }

        public ProfilePageViewModel()
        {
            GetUserCommand = new AsyncRelayCommand(GetUser);
        }

        private async Task GetUser()
        {
            IsBusy = true;
            await RouteServiceCall<UserRequestDto>.Get($"users/{getCurrentUserId()}", UserDataLoaded, UserDataFailed);
            IsBusy = false;
        }

        private string getCurrentUserId()
        {
            return "1";
        }
        private void UserDataLoaded(UserRequestDto userObj)
        {
            if (userObj != null)
            {
                User = userObj;
            }
            Message = "User data loaded";
        }

        private void UserDataFailed(Exception exception)
        {
            Message = exception?.Message;
        }
    }
}
