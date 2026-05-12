using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.NetworkSettings
{
    public class RouterNetworkConfiguration
    {
        public string Hostname { get; set; }
        public List<RouterNetworkInterface> Interfaces { get; set; }
    }
}
