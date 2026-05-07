using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface IDeviceRegistry
    {
        void Register(DeviceOnCanvas device);
        List<DeviceOnCanvas> GetAllDevices();
        DeviceOnCanvas GetById(string id);
  
    }
}
