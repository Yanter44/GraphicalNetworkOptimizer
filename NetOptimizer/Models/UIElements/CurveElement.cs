using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace NetOptimizer.Models.UIElements
{
    public class CurveElement : UIElementBase
    {
        public PointCollection Points { get; set; } = new();
    }
}
