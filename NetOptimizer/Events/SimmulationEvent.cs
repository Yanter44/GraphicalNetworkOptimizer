using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Events
{
    public class SimmulationEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PacketId { get; set; }
        public string FromDeviceId { get; set; }
        public string ToDeviceId { get; set; }
        public string? FromInterfaceId{ get; set; }
        public string? ToInterfaceId { get; set; }
        public PacketType PacketType { get; set; }
    }
}
