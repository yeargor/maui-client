using System.Collections.ObjectModel;

namespace MauiDemo2.Models.Route
{    
    public class RoutePostResponseDto
    {
        public RouteInfoResponseDto RouteInfo { get; set; }
        
        public string AuthorUsername { get; set; }
        
        public ICollection<PlaceInfoResponseDto> PlacesInfos { get; set; }
        
        public ICollection<AdditionalPlaceInfoResponseDto> AdditionalPlacesInfos { get; set; }
        
        public string RoutePathImageUrl { get; set; } //??
        
        public int RouteTime { get; set; } //??
        
        public ObservableCollection<Review> Reviews { get; set; }
        
        public ICollection<Models.Point> RoutePath { get; set; } //??
    }
}