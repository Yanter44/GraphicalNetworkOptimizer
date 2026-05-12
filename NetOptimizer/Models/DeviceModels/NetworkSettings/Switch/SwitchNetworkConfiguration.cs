using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.SubProperties
{
    public class SwitchNetworkConfiguration
    {
        public List<Vlan> Vlans { get; set; } = new();
        public List<SwitchNetworkInterface> Interfaces { get; set; } = new();
        public string Hostname { get; set; }
    }
}
