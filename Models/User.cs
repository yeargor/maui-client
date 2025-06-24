using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public ICollection<Subscription> subscriptions = new List<Subscription>();
    }
}
