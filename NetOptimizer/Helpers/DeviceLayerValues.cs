using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Helpers
{
    public static class DeviceLayerValues
    {
        public static Array All => Enum.GetValues(typeof(DeviceLayer));
    }
}
