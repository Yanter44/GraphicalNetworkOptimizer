using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.AddDeviceSettingsModels
{
    public class SwitchSettingModel : DeviceSettingsBase
    {
        public string Name { get; set; }
        public DeviceLayer DeviceLayer { get; set; }
        public int TotalPorts { get; set; }
        public int SfpPortsCount { get; set; }
        public bool SupportsPoe { get; set; }
    }
}
