using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;

namespace NetOptimizer.Models.DeviceModels
{
    public class RouterDevice : Device
    {
        public string Vendor { get; set; }
        public bool IsManaged { get; set; }
        public WifiOptions WifiOptions { get; set; } = new();
        public RouterPerformanceSpecs Performance { get; set; } = new();
        public RouterProtocolSupport ProtocolSupport { get; set; } = new();
        public decimal AveragePrice { get; set; }
        public RouterDevice(string name, RouterSettings settings) : base(name)
        {
            this.Type = DeviceType.Router;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.IsManaged = settings.IsManaged;
            this.AveragePrice = settings.AveragePrice;
            this.WifiOptions = settings.WifiOptions;
            this.Performance = settings.Performance;
            this.ProtocolSupport = settings.ProtocolSupport;
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

        public override IEnumerable<SimmulationEvent> ProcessPacket(Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}
