using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;

namespace NetOptimizer.Models.UIElements
{
    public class VisualLink
    {
        public Polyline Line { get; set; }
        public DeviceOnCanvas Source { get; set; }
        public DeviceOnCanvas Target { get; set; }
        public double Length { get; set; }
    }
}
