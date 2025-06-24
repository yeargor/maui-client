using System.Collections.ObjectModel;

namespace MauiDemo2.Models.Route
{    
    public class RoutePostResponseDto
    {
        public RouteInfoResponseDto RouteInfo { get; init; }
        
        public string AuthorUsername { get; init; }
        
        public ICollection<PlaceInfoResponseDto> PlacesInfos { get; init; }
        
        public ICollection<AdditionalPlaceInfoResponseDto> AdditionalPlacesInfos { get; init; }
        
        public int RouteDuration { get; init; }
        
        public ICollection<Review> Reviews { get; set; }
        
        public ICollection<Coordinate> RoutePath { get; init; }
    }
}