using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.SubProperties
{
    public class SwitchPerformanceSpecs
    {
        public decimal ThroughputGbps { get; set; }
        public int MacTableSize { get; set; }
        public int MaxVlans { get; set; }
    }
}
