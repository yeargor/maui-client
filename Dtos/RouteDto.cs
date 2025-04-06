using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Dtos
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TimesCompleted { get; set; }
        public double Rating { get; set; }
        public double Distance { get; set; }
        public string DaysAgo { get; set; }
        public bool IsFavorite { get; set; }
    }
}
