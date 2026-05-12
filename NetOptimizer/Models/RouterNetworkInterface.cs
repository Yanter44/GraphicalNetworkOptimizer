using NetOptimizer.Helpers;
using NetOptimizer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class RouterNetworkInterface : ILayer3Interface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string IpV4Address { get; set; }
        public string SubnetMask { get; set; }
        public string MacAddress { get; set; }
        public Port? PhysicalPort { get; set; }
        public RouterNetworkInterface()
        {
            IpV4Address = "0.0.0.0";
            SubnetMask = "0.0.0.0";
            IsEnabled = false;
            MacAddress = IpUtils.GenerateMac();
        }
    }
}
