using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class CanvasClipBoard
    {
        public List<DeviceOnCanvas> Devices { get; set; } = new();
        public List<UIElementBase> UIElements { get; set; } = new();
        public List<DeviceConnection> Connections { get; set; } = new();
    }
}
