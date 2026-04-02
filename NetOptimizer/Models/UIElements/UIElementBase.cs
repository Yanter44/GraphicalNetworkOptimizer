using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.UIElements
{
    public abstract class UIElementBase
    {
        public UIToolElementType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

    }
}
