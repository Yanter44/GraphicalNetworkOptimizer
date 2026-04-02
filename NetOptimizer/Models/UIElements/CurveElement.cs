using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NetOptimizer.Models.UIElements
{
    public class CurveElement : UIElementBase
    {
        public List<Point> Points { get; set; } = new();
    }
}
