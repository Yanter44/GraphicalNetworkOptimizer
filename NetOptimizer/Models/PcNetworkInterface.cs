using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class PcNetworkInterface
    {
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

            MacAddress = GenerateMac(); 
            IsEnabled = false;
        }
        private string GenerateMac()
        {
            var rand = new Random();
            return string.Join(":", Enumerable.Range(0, 6)
                .Select(_ => rand.Next(0, 256).ToString("X2")));
        }
    }
}
