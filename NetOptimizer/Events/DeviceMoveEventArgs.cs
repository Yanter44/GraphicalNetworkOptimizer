using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Events
{
    public class DeviceMoveEventArgs
    {
        public DeviceOnCanvas Device { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
    }
}
