using Microsoft.Maui.Storage;

namespace MauiDemo2.Services
{
    public static class AuthSessionService
    {
        private const string UserIdKey = "auth_user_id";
        private const string TokenKey = "auth_jwt_token";

        public static void SaveSession(int userId, string jwtToken)
        {
            Preferences.Set(UserIdKey, userId);
            Preferences.Set(TokenKey, jwtToken);
        }

        public static void ClearSession()
        {
            Preferences.Remove(UserIdKey);
            Preferences.Remove(TokenKey);
        }

        public static bool IsAuthenticated => Preferences.ContainsKey(UserIdKey) && Preferences.ContainsKey(TokenKey);

        public static int? GetUserId()
        {
            if (Preferences.ContainsKey(UserIdKey))
                return Preferences.Get(UserIdKey, 0);
            return null;
        }

        public static string? GetToken()
        {
            if (Preferences.ContainsKey(TokenKey))
                return Preferences.Get(TokenKey, string.Empty);
            return null;
        }
    }
}
