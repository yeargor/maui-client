using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Dtos
{
    public class TripResponseDto
    {
        //public User User { get; set; }

        public string Name { get; set; } = string.Empty;

        //public RouteType Type { get; set; }

        public string Theme { get; set; } = string.Empty;

        public string? Description { get; set; }

        public float Distance { get; set; }

        public TimeSpan Duration { get; set; }

        //public RouteDifficulty Difficulty { get; set; }

        public float Rating { get; set; }

        public string ListOfPoints { get; set; } = string.Empty;

        public DateTime DateUploaded { get; set; }

        //public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
