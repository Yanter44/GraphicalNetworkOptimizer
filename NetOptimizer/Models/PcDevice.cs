using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class PcDevice : Device
    {
        public PcDevice(string name, Action<PcDevice> configure = null)
            : base(name, 1) 
        {
            Type = DeviceType.PC;
            configure?.Invoke(this);
        }
    }
}
