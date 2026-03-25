using NetOptimizer.Models.DeviceModels.SubProperties;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.Dtos
{
    public class PcResponceDto
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public List<PortDto> Ports { get; set; }
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
        public decimal AveragePrice { get; set; }

    }
}
