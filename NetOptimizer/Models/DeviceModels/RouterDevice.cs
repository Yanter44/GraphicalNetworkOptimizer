using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.NetworkSettings;
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
        public RouterPerformanceSpecs PerformanceSpecs { get; set; } = new();
        public RouterProtocolSupport ProtocolSupport { get; set; } = new();
        public RouterNetworkConfiguration NetworkConfig { get; set; } = new();
        public RouterRuntimeState RuntimeState { get; set; }
        public decimal AveragePrice { get; set; }
        public RouterDevice(string name, RouterSettings settings) : base(name)
        {
            this.Type = DeviceType.Router;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.IsManaged = settings.IsManaged;
            this.AveragePrice = settings.AveragePrice;
            this.WifiOptions = settings.WifiOptions;
            this.PerformanceSpecs = settings.Performance;
            this.ProtocolSupport = settings.ProtocolSupport;
            this.NetworkConfig = new RouterNetworkConfiguration()
            {
                Hostname = "Router",
                Interfaces = new List<RouterNetworkInterface>()
            };
            GeneratePortsAndInterfaces(settings.Ports);
            ConfigureInterfaces();
        }

        private void GeneratePortsAndInterfaces(List<PortDto> portDtos)
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

                    var prefix = GetInterfacePrefix(dto.Speed, dto.Type);
                    var portId = $"{slot}/{port}";

                    var portEntity = new Port
                    {
                        PortName = dto.Type.ToString(),
                        PortNumber = portId,
                        Type = dto.Type,
                        Owner = this,
                    };
                    this.Ports.Add(portEntity);
                    this.NetworkConfig.Interfaces.Add(new RouterNetworkInterface
                    {
                        Name = $"{prefix}{portId}",
                        PhysicalPort = portEntity
                    });
                }
            }
        }
        private void ConfigureInterfaces()
        {
            foreach (var interfacee in this.NetworkConfig.Interfaces)
            {
                interfacee.IsEnabled = false;
            }
        }
        private string GetInterfacePrefix(string speed, PortType type)
        {
            return (speed, type) switch
            {
                ("100M", PortType.RJ45) => "Fa",
                ("1G", PortType.RJ45) => "Gi",
                ("10G", _) => "Te",
                ("40G", _) => "Fo",
                _ => "Gi"
            };
        }
        public override IEnumerable<SimmulationEvent> ProcessPacket(Packet packet, string InPortId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<INetworkInterface> GetInterfaces() => NetworkConfig.Interfaces;
        public override IEnumerable<Port> GetPorts() => Ports;
    }
}
