using MauiDemo2.Models.Route;

namespace MauiDemo2.Models
{
    public class UserProfileResponseDto
    {
        public required UserInfoResponseDto UserInfo { get; init; }
        
        public int Distance { get; init; }
        
        public required int SubscriptionsNumber { get; init; }
        
        public required int FollowersNumber { get; init; }
    }

    public class UserInfoResponseDto
    {
        public required int UserId { get; init; }
        
        public required string Username { get; init; }
            
        public string? LastName { get; init; }
            
        public string? FirstName { get; init; }
            
        public string? SecondName { get; init; }
            
        public string? ImageUrl { get; init; }
    }
}
