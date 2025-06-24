using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiDemo2.Services
{
    public class AuthService
    {
        private readonly string _loginUrl = "https://byways-p378.onrender.com/api/auth/login";
        private readonly UserService _userService;
        private readonly CurrentUserService _currentUserService;

        public AuthService(UserService userService, CurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        public async Task<(bool Success, string? Token, string? Error)> LoginAsync(string email, string password)
        {
            using var client = new HttpClient();
            var loginData = new { Email = email, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(_loginUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseString);
                    var root = doc.RootElement;
                    var token = root.TryGetProperty("token", out var t) ? t.GetString() : null;
                    if (!string.IsNullOrEmpty(token))
                    {
                        Preferences.Set("jwtToken", token);
                        Preferences.Set("login", email);
                        Preferences.Set("password", password);
                        return (true, token, null);
                    }
                    else
                    {
                        return (false, null, "Ошибка: токен не получен");
                    }
                }
                else
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseString);
                        var root = doc.RootElement;
                        var error = root.TryGetProperty("error", out var e) ? e.GetString() : responseString;
                        return (false, null, error);
                    }
                    catch
                    {
                        return (false, null, responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> LoginAndSetCurrentUserAsync(string email, string password)
        {
            var (success, token, error) = await LoginAsync(email, password);
            if (!success || string.IsNullOrEmpty(token))
                return (false, error);

            var userId = JwtService.GetUserIdFromToken(token);
            if (userId == null)
                return (false, "UserId not found in token");

            var userProfile = await _userService.GetUserProfileByIdAsync(userId, token);
            if (userProfile == null)
                return (false, "User profile not found");

            _currentUserService.SetCurrentUser(userProfile);
            return (true, null);
        }
    }
}
