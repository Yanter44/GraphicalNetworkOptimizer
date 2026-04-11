using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NetOptimizer.Views.Selectors
{
    public class UIElementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RectangleTemplate { get; set; }
        public DataTemplate EllipseTemplate { get; set; }
        public DataTemplate LineTemplate { get; set; }
        public DataTemplate CurveTemplate { get; set; }
        public DataTemplate LabelTemplate { get; set; }
        public DataTemplate ArrowTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                RectangleElement => RectangleTemplate,
                EllipseElement => EllipseTemplate,
                LineElement => LineTemplate,
                CurveElement => CurveTemplate,
                LabelElement => LabelTemplate,
                ArrowElement => ArrowTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
