using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class PcDevice : Device
    {
        public PcDevice(string name,PcSettings settings) : base(name)
        {
            this.Type = DeviceType.PC;
            this.DeviceModel = "Generic PC";

            Ports.Add(new Port
            {
                PortNumber = "Fa0/0",
                Type = PortType.RJ45,
                Owner = this
            });
        }
    }
}
