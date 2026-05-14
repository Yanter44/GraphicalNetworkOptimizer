using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class PendingPacket
    {
        public Packet Packet { get; set; }
        public string NextHopIp { get; set; }
        public string InterfaceId { get; set; }

    }
}
