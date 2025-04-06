using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class Card
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int TimesCompleted { get; set; } // Количество пройденных раз
        public double Rating { get; set; } // Оценка
        public double Distance { get; set; } // Дистанция в км

        public string DaysAgo; // Количество дней назад
    }
}
