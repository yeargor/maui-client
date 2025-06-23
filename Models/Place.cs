using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class Place : LocationBase
    {
        public int? OrderOfVisit { get; set; }
        public bool IsCompleted { get; set; }
    }
}
