using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class LocationPin
    {
        public string? Description { get; set; }
        public string? Address { get; set; }
        public Location? Location { get; set; }
        public ImageSource? ImageSource { get; set; }
        public int? OrderOfVisit { get; set; }
        public int? LocationBaseId {get; set;}
    }
}
