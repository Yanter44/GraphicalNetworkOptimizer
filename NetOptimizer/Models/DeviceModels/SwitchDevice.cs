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

        public override IEnumerable<SimmulationEvent> ProcessPacket(Packet packet, string InPortId)
        {
            SwitchNetworkInterface switchInterface;
            switchInterface = NetworkConfig.Interfaces.FirstOrDefault(i => i.PhysicalPort.Id == InPortId);

            if (switchInterface == null || !switchInterface.IsEnabled)
                yield break;

            if (packet is ArpPacket arp)
            {
                foreach (var ev in HandleArp(arp, switchInterface))
                    yield return ev;

                yield break;
            }
            if(packet is IcmpPacket icmp)
            {
                foreach (var ev in HandleIcmp(icmp, switchInterface))
                    yield return ev;

                yield break;
            }
        }
        private IEnumerable<SimmulationEvent> HandleIcmp(IcmpPacket icmp,SwitchNetworkInterface inIface)
        {
            var vlan = inIface.AccessVlan;

            var macEntry = RuntimeState.MacTable
                .FirstOrDefault(x =>
                    x.MacAddress == icmp.DestinationMac &&
                    x.VlanId == vlan);

            var outIface = NetworkConfig.Interfaces
                .FirstOrDefault(i =>
                    i.Name == macEntry.InterfaceName);

            if (outIface == null)
                yield break;

            var nextDevice = outIface.PhysicalPort.ConnectedTo?.Owner;

            if (nextDevice == null)
                yield break;

            yield return new SimmulationEvent
            {
                Packet = icmp,

                FromDeviceId = this.Id,
                ToDeviceId = nextDevice.Id,

                FromPortId = outIface.PhysicalPort.Id,
                ToPortId = outIface.PhysicalPort.ConnectedTo.Id
            };
        }
        private IEnumerable<SimmulationEvent> HandleArp(ArpPacket arp, SwitchNetworkInterface inIface)
        {
            var existing = RuntimeState.MacTable.FirstOrDefault(e => e.MacAddress == arp.SourceMac);

            if (existing == null)
            {
                RuntimeState.MacTable.Add(new MacTableEntry
                {
                    MacAddress = arp.SourceMac,
                    InterfaceName = inIface.Name,
                    VlanId = (int)inIface.AccessVlan,
                    Type = MacEntryType.Dynamic
                });
            }
            else
            {
                existing.InterfaceName = inIface.Name;
            }

            if (arp.ArpType == ArpType.Request)
            {
                foreach (var ev in FloodArpRequest(arp, inIface))
                    yield return ev;

                yield break;
            }

            if (arp.ArpType == ArpType.Response)
            {
                foreach (var ev in ForwardArpResponse(arp, inIface))
                    yield return ev;

                yield break;
            }
        }
        private IEnumerable<SimmulationEvent> FloodArpRequest(ArpPacket arp, SwitchNetworkInterface inIface)
        {
            foreach (var iface in NetworkConfig.Interfaces)
            {
                if (!iface.IsEnabled)
                    continue;

                if (iface.PhysicalPort.Id == inIface.PhysicalPort.Id)
                    continue;

                if (iface.AccessVlan != inIface.AccessVlan)
                    continue;

                var nextDevice = iface.PhysicalPort.ConnectedTo?.Owner;

                if (nextDevice == null)
                    continue;

                yield return new SimmulationEvent
                {
                    Packet = arp,

                    FromDeviceId = this.Id,
                    ToDeviceId = nextDevice.Id,

                    FromPortId = iface.PhysicalPort.Id,
                    ToPortId = iface.PhysicalPort.ConnectedTo.Id
                };
            }
        }
        private IEnumerable<SimmulationEvent> ForwardArpResponse(ArpPacket arp, SwitchNetworkInterface inIface)
        {
            var destinationEntry = RuntimeState.MacTable
                .FirstOrDefault(x =>
                    x.MacAddress == arp.DestinationMac &&
                    x.VlanId == (int)inIface.AccessVlan);

            if (destinationEntry == null)
                yield break;

            var outIface = NetworkConfig.Interfaces
                .FirstOrDefault(i =>
                    i.Name == destinationEntry.InterfaceName);

            if (outIface == null)
                yield break;

            var nextDevice = outIface.PhysicalPort.ConnectedTo?.Owner;

            if (nextDevice == null)
                yield break;

            yield return new SimmulationEvent
            {
                Packet = arp,

                FromDeviceId = this.Id,
                ToDeviceId = nextDevice.Id,

                FromPortId = outIface.PhysicalPort.Id,
                ToPortId = outIface.PhysicalPort.ConnectedTo.Id
            };
        }
        public override IEnumerable<INetworkInterface> GetInterfaces() => NetworkConfig.Interfaces;
        public override IEnumerable<Port> GetPorts() => Ports;
    }

}
