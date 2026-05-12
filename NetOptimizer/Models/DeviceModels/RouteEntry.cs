using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class RouteEntry
    {       
        public string DestinationNetwork { get; set; }
        public string SubnetMask { get; set; }
        public string NextHopIp { get; set; }
    }
}
