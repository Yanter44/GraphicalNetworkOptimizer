using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.NetworkSettings
{
    public class SwitchRuntimeState
    {
        public List<MacTableEntry> MacTable { get; set; } = new();

    }
}
