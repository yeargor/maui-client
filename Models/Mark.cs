using MauiDemo2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class Mark
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public MarkType MarkType { get; set; }
    }
}
