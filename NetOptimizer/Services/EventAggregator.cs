using NetOptimizer.Models;
using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Services
{
    public class EventAggregator
    {
        public static EventAggregator Instance { get; } = new EventAggregator();

        public event Action<DeviceToAddDto, DeviceSettingsBase> DeviceCreated;

        public void PublishDeviceCreated(DeviceToAddDto dto, DeviceSettingsBase settings)
            => DeviceCreated?.Invoke(dto, settings);
    }
}
