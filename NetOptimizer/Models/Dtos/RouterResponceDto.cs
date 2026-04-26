using NetOptimizer.Models.DeviceModels.SubProperties;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.Dtos
{
    public class RouterResponceDto
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public bool IsManaged { get; set; }
        public int TotalPorts { get; set; }
        public List<PortDto> Ports { get; set; } = new();
        public WifiOptions WifiOptions { get; set; } = new();
        public RouterPerformanceSpecs Performance { get; set; } = new();
        public RouterProtocolSupport ProtocolSupport { get; set; } = new();
        public decimal AveragePrice { get; set; }
    }
}
