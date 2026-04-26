using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.SubProperties
{
    public class RouterProtocolSupport
    {
        public bool SupportsOspf { get; set; }
        public bool SupportsVrrp { get; set; }
        public bool SupportsIpsec { get; set; }
        public bool SupportsNat { get; set; }
    }
}
