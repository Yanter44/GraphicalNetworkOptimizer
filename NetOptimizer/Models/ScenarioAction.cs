using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class ScenarioAction
    {
        public string SourceDeviceId { get; set; }
        public string TargetDeviceId { get; set; }  
        public PacketType PacketType { get; set; }
    }
}
