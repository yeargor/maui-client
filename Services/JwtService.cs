using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MauiDemo2.Services
{
    public static class JwtService
    {
        public static int? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");
            if (userIdClaim == null)
                return null;
            if (int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        public static string? GetJwtToken()
        {
            return Microsoft.Maui.Storage.Preferences.Get("jwtToken", string.Empty);
        }
    }
}
