using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class ArpPacket : Packet
    {
        public ArpType ArpType { get; set; }
    }
}
