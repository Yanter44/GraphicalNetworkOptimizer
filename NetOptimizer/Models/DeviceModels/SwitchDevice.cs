using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.NetworkSettings;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;

namespace NetOptimizer.Models.DeviceModels
{
    public class SwitchDevice : Device
    {
        public string Vendor { get; set; }
        public DeviceLayer Layer { get; set; }
        public PoeSpecs PoeSpecs { get; set; }
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; }
        public SwitchProtocolSupport ProtocolSupport { get; set; }
        public SwitchNetworkConfiguration NetworkConfig { get; set; }
        public SwitchRuntimeState RuntimeState { get; set; } = new();
        public decimal AveragePrice { get; set; }
        public SwitchRoleType SwitchRoleType { get; set; }
        public SwitchDevice(string name, SwitchSettings settings) : base(name)
        {
            this.Type = DeviceType.Switch;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.Vendor = settings.Vendor;
            this.Layer = settings.DeviceLayer;
            this.PoeSpecs = settings.PoeSpecs;
            this.PerformanceSpecs = settings.PerformanceSpecs;
            this.ProtocolSupport = settings.ProtocolSupport;
            this.SwitchRoleType = settings.SwitchRoleType;
            this.AveragePrice = settings.AveragePrice;
            this.NetworkConfig = new SwitchNetworkConfiguration
            {
                Hostname = name,
                Vlans = new List<Vlan> { new Vlan { Id = 1, Name = "DefaultVlan" } },
                Interfaces = new List<SwitchNetworkInterface>()
            };
            GeneratePortsAndInterfaces(settings.Ports);
            ConfigureInterfaces();
        }

        private void GeneratePortsAndInterfaces(List<PortDto> portDtos)
        {
            var counters = new Dictionary<PortType, int>();

            foreach (var dto in portDtos)
            {
                if (!counters.ContainsKey(dto.Type)) counters[dto.Type] = 0;

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
                    this.NetworkConfig.Interfaces.Add(new SwitchNetworkInterface
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
                interfacee.IsEnabled = true;
                interfacee.SwitchPortMode = SwitchPortMode.Access;
                interfacee.AccessVlan = 1;
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

        public override IEnumerable<SimmulationEvent> ProcessPacket(Packet packet)
        {
            throw new NotImplementedException();
        }
    }

}
