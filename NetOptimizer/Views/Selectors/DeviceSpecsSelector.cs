using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.DeviceParametrsViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NetOptimizer.Views.Selectors
{
    public class DeviceSpecsSelector : DataTemplateSelector
    {
        public DataTemplate PcTemplate { get; set; }
        public DataTemplate SwitchTemplate { get; set; }
        public DataTemplate RouterTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DeviceViewModelBase vm)
            {
                return vm switch
                {
                    PcDeviceViewModel => PcTemplate,
                    SwitchDeviceViewModel => SwitchTemplate,
                    RouterDeviceViewModel => RouterTemplate,
                    _ => null
                };
            }
            return null;
        }
    }
}
