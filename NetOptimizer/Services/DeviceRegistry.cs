using NetOptimizer.Interfaces;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.MainWindoww;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Services
{
    public class DeviceRegistry : IDeviceRegistry
    {
        private readonly Dictionary<string, DeviceOnCanvas> _devices = new();

        public void Register(DeviceOnCanvas device)
        {
            _devices[device.LogicDevice.Id] = device;
        }

        public List<DeviceOnCanvas> GetAllDevices()
        {
            return _devices.Values.ToList();
        }

        public DeviceOnCanvas GetById(string id)
        {
            return _devices.TryGetValue(id, out var d) ? d : null;
        }
    }
}
