using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.NetworkSettings
{
    public class PcNetworkConfiguration
    {
        public string Hostname { get; set; }
        public List<PcNetworkInterface> Interfaces { get; set; }
    }
}
