using MauiDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Services
{
    public class SubscriptionService
    {
        private List<Subscription> _subscriptions;

        public SubscriptionService()
        {
            _subscriptions = GenerateSubscriptions();
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync()
        {
            return _subscriptions;
        }

        private List<Subscription> GenerateSubscriptions()
        {
            return new List<Subscription>
            {
                new Subscription { UserId = 1, FollowedUserId = 2 },
                new Subscription { UserId = 2, FollowedUserId = 1 },
                new Subscription { UserId = 1, FollowedUserId = 3 }
            };
        }
    }
}
