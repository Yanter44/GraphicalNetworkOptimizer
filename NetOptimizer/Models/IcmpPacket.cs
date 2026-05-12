using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class IcmpPacket : Packet
    {
        public IcmpType IcmpType { get; set; }
        public ushort Sequence { get; set; }
    }
}
