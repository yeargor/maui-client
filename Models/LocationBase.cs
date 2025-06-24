using MauiDemo2.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public abstract class LocationBase
    {
        public int Id { get; set; }

        public int PointId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Point Point { get; set; }

        [JsonIgnore]
        public ICollection<RouteResponseDto> Routes { get; set; }
    }
}
