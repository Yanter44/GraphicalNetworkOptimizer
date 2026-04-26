using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.DeviceSettings
{
    public class SwitchSettings : DeviceSettingsBase
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public List<PortDto> Ports { get; set; } = new List<PortDto>();
        public PoeSpecs PoeSpecs { get; set; }
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; }
        public SwitchProtocolSupport ProtocolSupport { get; set; }
        public DeviceLayer DeviceLayer { get; set; }
        public SwitchRoleType SwitchRoleType { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
