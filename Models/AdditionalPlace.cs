using MauiDemo2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class AdditionalPlace : LocationBase
    {
        public AdditionalPlaceType Type { get; set; }
    }
}
