using MediatR;
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
        private readonly IMediator _mediator;

        public DeviceViewModelFactory(
            IWindowNavigator navigator,
            IMediator mediator)
        {
            _navigator = navigator;
            _mediator = mediator;
        }

        public DeviceViewModelBase Create(DeviceOnCanvas device)
        {
            return device.LogicDevice switch
            {
                PcDevice => new PcDeviceViewModel(device, _mediator),
                SwitchDevice => new SwitchDeviceViewModel(device, _navigator),
                RouterDevice => new RouterDeviceViewModel(device, _navigator, _mediator),
                _ => throw new NotSupportedException()
            };
        }
    }
}
