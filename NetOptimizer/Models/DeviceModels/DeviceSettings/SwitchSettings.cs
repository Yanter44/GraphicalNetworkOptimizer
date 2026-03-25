using NetOptimizer.Enums;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.DeviceSettings
{
    public class SwitchSettings : DeviceSettingsBase
    {
        public string Model { get; set; }
        public string Vendor { get; set; }
        public int TotalPorts { get; set; }
        public bool SupportsPoe { get; set; }
        public List<PortDto> Ports { get; set; } = new List<PortDto>();
        public decimal AveragePrice { get; set; }
        public DeviceLayer DeviceLayer { get; set; }
    }
}
