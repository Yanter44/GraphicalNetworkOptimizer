using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class SwitchNetworkInterface
    {
        public SwitchPortMode SwitchPortMode;
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public Port? PhysicalPort { get; set; }
        public int? AccessVlan { get; set; }
        public List<int>? AllowedVlans { get; set; }
    }
}
