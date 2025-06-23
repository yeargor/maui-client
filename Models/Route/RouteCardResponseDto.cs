namespace MauiDemo2.Models.Route;

public class RouteCardResponseDto
{
    public RouteInfoResponseDto RouteInfo { get; set; }
    
    public UserInfoResponseDto UserInfo { get; set; }
    
    public int UserCount { get; set; }
}