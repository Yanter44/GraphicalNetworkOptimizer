using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class Packet
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string SourceDeviceId { get; set; }
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public PacketType Type { get; set; }
        public int TTL { get; set; } = 20;
    }
}
