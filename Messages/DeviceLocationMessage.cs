using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Messages
{
    public class DeviceLocationMessage : ValueChangedMessage<DeviceLocation>
    {
        public DeviceLocationMessage(DeviceLocation deviceLocation) : base(deviceLocation) { }
    }
}
