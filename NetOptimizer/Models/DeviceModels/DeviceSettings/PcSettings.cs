using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;

namespace NetOptimizer.Models.DeviceModels.DeviceSettings
{
    public class PcSettings : DeviceSettingsBase
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public List<PortDto> Ports { get; set; }
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
        public decimal AveragePrice { get; set; }

    }
}
