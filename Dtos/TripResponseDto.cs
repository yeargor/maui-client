using MauiDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Dtos
{
    public class TripResponseDto
    {
        public int Id { get; set; }

        //public int UserId { get; set; }

        public User User { get; set; } = null!;

        public string Name { get; set; }

        //public RouteType Type { get; set; }

        public string Theme { get; set; }

        public string? Description { get; set; }

        public float Length { get; set; }

        //public TimeSpan Duration { get; set; }

        //public RouteDifficulty Difficulty { get; set; }

        public float Rating { get; set; }

        //public DateTime DateUploaded { get; set; }

        public ICollection<Place> Places { get; set; }

        public ICollection<AdditionalPlace> AdditionalPlaces { get; set; }
    }
}
