using MediatR;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Commands
{
    public class CreateDeviceCommand : IRequest
    {
        public DeviceToAddDto Device { get; set; }
        public DeviceSettingsBase Settings { get; set; }
    }
}
