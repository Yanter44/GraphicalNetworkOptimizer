using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.AddDeviceSettingsModels
{
    public class PrinterSettingModel : DeviceSettingsBase
    {
        public string Name { get; set; }
        public AccessToInternetType AccessToInternetType { get; set; }
        
    }
}
