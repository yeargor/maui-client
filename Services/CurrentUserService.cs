using MauiDemo2.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;

namespace MauiDemo2.Services
{
    public class CurrentUserService
    {
        private UserProfileResponseDto? _currentUser;
        
        public UserProfileResponseDto GetCurrentUser()
        {
            if (_currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("[CurrentUserService] Current user is not set.");
                throw new InvalidOperationException("Current user is not set.");
            }
            System.Diagnostics.Debug.WriteLine($"[CurrentUserService] GetCurrentUser: {_currentUser.UserInfo.Username} (ID: {_currentUser.UserInfo.UserId})");
            return _currentUser;
        }

        public void SetCurrentUser(UserProfileResponseDto userProfile)
        {
            _currentUser = userProfile;
            System.Diagnostics.Debug.WriteLine($"[CurrentUserService] SetCurrentUser: {_currentUser?.UserInfo?.Username ?? "null"} (ID: {_currentUser?.UserInfo?.UserId.ToString() ?? "null"})");
        }
    }
}
