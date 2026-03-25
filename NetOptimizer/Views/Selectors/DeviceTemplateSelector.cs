using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NetOptimizer.Views.Selectors
{
    public class DeviceTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WithCountTemplate { get; set; }
        public DataTemplate WithoutCountTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var device = item as AvailableDevicesForEditorDto;

            if (device == null)
                return base.SelectTemplate(item, container);

            return device.HasCount
                ? WithCountTemplate
                : WithoutCountTemplate;
        }
    }
}
