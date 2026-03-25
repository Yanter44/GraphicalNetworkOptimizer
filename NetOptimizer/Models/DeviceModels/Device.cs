using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public abstract class Device
    {
        public string Name { get; protected set; }
        public string DeviceModel { get; set; }
        public string Vendor { get; set; } 
        public DeviceType Type { get; protected set; }
        public List<Port> Ports { get; } = new List<Port>();

        protected Device(string name) => Name = name;
    }
}
