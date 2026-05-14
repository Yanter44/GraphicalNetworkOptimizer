using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.NetworkSettings
{
    public class RouterRuntimeState
    {
        public List<ArpTableEntry> ArpTable { get; set; } = new();
        public List<StaticRouteEntry> StaticRoutingTable { get; set; } = new();
        public List<AutomaticRoutingEntry> AutomaticRoutingTable { get; set; } = new();
        public Queue<PendingPacket> PendingPackets { get; set; } = new();
        public HashSet<string> PendingArpRequests { get; set; } = new();
    }
}
