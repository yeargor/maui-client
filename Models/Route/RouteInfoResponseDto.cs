using MauiDemo2.Models.Common;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDemo2.Models.Route
{  
    public partial class RouteInfoResponseDto : ObservableObject
    {
        public int RouteId { get; set; }
        
        public int UserId { get; set; }
        
        public string RouteTitle { get; set; }
        
        private string _routeDescription;
        public string RouteDescription
        {
            get => _routeDescription;
            set
            {
                _routeDescription = value;
                DescriptionParts = SplitDescription(_routeDescription, 100);
            }
        }

        public List<string> DescriptionParts { get; private set; } = new List<string>();

        private static List<string> SplitDescription(string description, int partLength)
        {
            var parts = new List<string>();
            if (string.IsNullOrEmpty(description)) return parts;
            for (int i = 0; i < description.Length; i += partLength)
            {
                parts.Add(description.Substring(i, Math.Min(partLength, description.Length - i)));
            }
            return parts;
        }
        
        public RouteType RouteType { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        private float _routeDistance;
        public float RouteDistance
        {
            get => _routeDistance;
            set
            {
                _routeDistance = (float)Math.Round(value, 2);
            }
        }
        
        private float _routeRating;
        public float RouteRating
        {
            get => _routeRating;
            set => _routeRating = (float)Math.Round(value, 2);
        }
        
        [ObservableProperty]
        private UserLike? userLike;
        public List<string> ImagesUrls { get; set; } = new List<string>();
    }
}