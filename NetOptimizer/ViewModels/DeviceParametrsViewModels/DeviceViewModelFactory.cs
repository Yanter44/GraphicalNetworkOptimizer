using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public class DeviceViewModelFactory
    {
        private readonly IWindowNavigator _navigator;

        public DeviceViewModelFactory(IWindowNavigator navigator)
        {
            _navigator = navigator;
        }
        public DeviceViewModelBase Create(DeviceOnCanvas device)
        {
            return device.LogicDevice switch
            {
                PcDevice pc => new PcDeviceViewModel(device),
                SwitchDevice => new SwitchDeviceViewModel(device, _navigator),
                RouterDevice => new RouterDeviceViewModel(device),
                _ => throw new NotSupportedException()
            };
        }
    }
}
