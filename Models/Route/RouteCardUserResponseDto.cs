using MauiDemo2.Models.Common;

namespace MauiDemo2.Models.Route;

public class RouteCardUserResponseDto
{
    public RouteInfoResponseDto RouteInfo { get; set; }
    
    public List<UserMark> UserMarks { get; set; }
    
    public UserInfoResponseDto UserInfo { get; set; }
    
    public int UserCount { get; set; }
}