using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class RouteMainPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                DescriptionParts = SplitText(value, 200);
            }
        }
        public List<string> DescriptionParts { get; private set; } = new List<string>();
        public int TimesCompleted { get; set; }
        public double Rating { get; set; }
        public double Distance { get; set; }
        public string DaysAgo { get; set; }
        public bool IsFavorite { get; set; }
        private string _description;

        public List<string> Images { get; set; } = new List<string>
        {
            "white.jpg",
            "white.jpg",
            "white.jpg"
        };
        private List<string> SplitText(string text, int chunkSize)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            var parts = new List<string>();
            for (int i = 0; i < text.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, text.Length - i);
                parts.Add(text.Substring(i, length));
            }
            return parts;
        }
    }
}
