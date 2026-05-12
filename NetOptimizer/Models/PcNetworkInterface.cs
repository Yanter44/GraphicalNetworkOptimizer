using NetOptimizer.Helpers;
using NetOptimizer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class PcNetworkInterface : ILayer3Interface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public Port? PhysicalPort { get; set; }
        public string IpV4Address { get; set; }
        public string SubnetMask { get; set; }
        public string DefaultGateway { get; set; }
        public string DNS { get; set; }
        public string MacAddress { get; set; }
        public PcNetworkInterface()
        {
            IpV4Address = "0.0.0.0";
            SubnetMask = "0.0.0.0";
            DefaultGateway = "0.0.0.0";
            DNS = "0.0.0.0";

            MacAddress = IpUtils.GenerateMac(); 
            IsEnabled = false;
        }
    }
}
