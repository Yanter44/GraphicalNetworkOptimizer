using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.SubProperties
{
    public class SwitchProtocolSupport
    {
        public bool SupportsLag { get; set; }
        public bool SupportsLacp { get; set; }
        public bool SupportsLoopProtection { get; set; }
    }
}
