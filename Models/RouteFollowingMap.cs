using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class RouteFollowingMap
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Place> Places { get; set; } = new();
        public List<AdditionalPlace> AdditionalPlaces { get; set; } = new();
    }
}
