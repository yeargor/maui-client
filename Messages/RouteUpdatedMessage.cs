using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiDemo2.Models;
using MauiDemo2.Models.Common;

namespace MauiDemo2.Messages
{
    public class RouteUpdatedMessage : ValueChangedMessage<(int RouteId, UserLike? UserLike)>
    {
        public RouteUpdatedMessage(int routeId, UserLike? userLike)
            : base((routeId, userLike)) { }
    }
}
