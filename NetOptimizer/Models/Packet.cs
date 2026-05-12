using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public abstract class Packet
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SourceMac { get; set; }
        public string DestinationMac { get; set; }
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int TTL { get; set; } = 20;
    }
}
