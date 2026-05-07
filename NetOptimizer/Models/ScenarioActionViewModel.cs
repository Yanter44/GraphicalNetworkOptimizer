using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class ScenarioActionViewModel
    {
        public DeviceOnCanvas SourceDevice { get; set; }
        public DeviceOnCanvas TargetDevice { get; set; }
        public PacketType PacketType { get; set; }
    }
}
