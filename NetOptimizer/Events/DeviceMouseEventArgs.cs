using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NetOptimizer.Events
{
    public class DeviceMouseEventArgs
    {
        public DeviceOnCanvas Device { get; set; }
        public DeviceMouseAction Action { get; set; }
        public Point Position { get; set; }
    }
}
