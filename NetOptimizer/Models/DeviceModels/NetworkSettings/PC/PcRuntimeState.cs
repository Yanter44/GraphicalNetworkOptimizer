using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.NetworkSettings
{
    public class PcRuntimeState
    {
        public List<ArpTableEntry> ArpTable { get; set; } = new();
        public Queue<Packet> PendingPackets { get; set; } = new();
        public HashSet<string> PendingArpRequests { get; set; } = new();
    }
}
