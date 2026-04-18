using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.Dtos;

namespace NetOptimizer.Models.DeviceModels
{
    public class SwitchDevice : Device
    {
        public string Vendor { get; set; }
        public DeviceLayer Layer { get; set; }
        public bool SupportsPoe { get; set; }
        public decimal AveragePrice { get; set; }

        public SwitchDevice(string name, SwitchSettings settings) : base(name)
        {
            this.Type = DeviceType.Switch;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.Vendor = settings.Vendor;
            this.Layer = settings.DeviceLayer;
            this.SupportsPoe = settings.SupportsPoe;
            this.AveragePrice = settings.AveragePrice;
            GeneratePorts(settings.Ports);
        }

        private void GeneratePorts(List<PortDto> portDtos)
        {
            var counters = new Dictionary<PortType, int>();

            foreach (var dto in portDtos)
            {
                if (!counters.ContainsKey(dto.Type))
                    counters[dto.Type] = 0;

                for (int i = 0; i < dto.Count; i++)
                {
                    int index = counters[dto.Type]++;

                    int slot = index / 10;
                    int port = index % 10;

                    Ports.Add(new Port
                    {
                        PortName = $"{dto.Type}",
                        PortNumber = $"{slot}/{port}",
                        Type = dto.Type,
                        Owner = this
                    });
                }
            }
        }
    }

}
