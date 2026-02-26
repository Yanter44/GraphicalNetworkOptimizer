using NetOptimizer.Enums;

namespace NetOptimizer.Models.AddDeviceSettingsModels
{
    public class PrinterSettingModel : DeviceSettingsBase
    {
        public string Name { get; set; }
        public AccessToInternetType AccessToInternetType { get; set; }

    }
}
