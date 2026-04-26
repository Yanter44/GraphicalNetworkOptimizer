using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.SubProperties
{
    public class SwitchNetworkConfiguration
    {
        public ObservableCollection<Vlan> Vlans { get; set; } = new();

        public ObservableCollection<NetworkInterface> Interfaces { get; set; } = new();
        public string Hostname { get; set; }
    }
}
