using Microsoft.VisualBasic.Devices;
using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Helpers;
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
        public RouterRuntimeState RuntimeState { get; set; } = new();
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
        public RouteAddResult TryAddStaticRoute(StaticRouteEntry entry)
        {
            var iface = FindInterfaceByNextHop(entry.NextHopIp);

            if (iface == null)
            {
                return new RouteAddResult
                {
                    Success = false,
                    Error = "Следующий узел недостижим"
                };
            }

            entry.InterfaceId = iface.Id;
            RuntimeState.StaticRoutingTable.Add(entry);

            return new RouteAddResult
            {
                Success = true,
                InterfaceName = iface.Name
            };
        }
        public void RebuildRoutingTable()
        {
            RuntimeState.AutomaticRoutingTable.Clear();
            RebuildAutomaticRoutes();
        }
        private void RebuildAutomaticRoutes()
        {
            foreach (var iface in NetworkConfig.Interfaces)
            {
                if (!iface.IsEnabled)
                    continue;

                if (string.IsNullOrEmpty(iface.IpV4Address) ||
                    string.IsNullOrEmpty(iface.SubnetMask))
                    continue;

                RuntimeState.AutomaticRoutingTable.Add(
                    new AutomaticRoutingEntry
                    {
                        Network = iface.IpV4Address,
                        SubnetMask = iface.SubnetMask,
                        InterfaceId = iface.Id
                    });
            }
        }
        private RouterNetworkInterface FindInterfaceByNextHop(string nextHopIp)
        {
            var iface = NetworkConfig.Interfaces.FirstOrDefault(i => i.IsEnabled &&
                                                                     IpUtils.IsSameSubnet(nextHopIp, i.IpV4Address, i.SubnetMask));
            return iface;
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
            RouterNetworkInterface routerInterface;
            routerInterface = NetworkConfig.Interfaces.FirstOrDefault(i => i.PhysicalPort.Id == InPortId);

            if (routerInterface == null || !routerInterface.IsEnabled)
                yield break;

            if (packet is ArpPacket arp)
            {
                foreach (var ev in HandleArp(arp, routerInterface))
                    yield return ev;

                yield break;
            }
            if (packet is IcmpPacket icmp)
            {
                foreach (var ev in HandleIcmp(icmp, routerInterface))
                    yield return ev;

                yield break;
            }
        }
        private IEnumerable<SimmulationEvent> HandleArp(ArpPacket arp, RouterNetworkInterface iface)
        {
            var nextDevice = iface.PhysicalPort.ConnectedTo.Owner;

            if (arp.DestinationIp == iface.IpV4Address)
            {
                if(arp.ArpType == ArpType.Request)
                {
                    var reply = new ArpPacket
                    {
                        SourceMac = iface.MacAddress,
                        DestinationMac = arp.SourceMac,
                        ArpType = ArpType.Response,
                        SourceIp = iface.IpV4Address,
                        DestinationIp = arp.SourceIp
                    };
                    RuntimeState.ArpTable.Add(new ArpTableEntry
                    {
                        IpAddress = arp.SourceIp,
                        MacAddress = arp.SourceMac
                    });

                    yield return new SimmulationEvent
                    {
                        Packet = reply,
                        FromDeviceId = this.Id,
                        ToDeviceId = nextDevice.Id,

                        FromPortId = iface.PhysicalPort.Id,
                        ToPortId = iface.PhysicalPort.ConnectedTo.Id
                    };
                }
                else if (arp.ArpType == ArpType.Response)
                {
                    RuntimeState.ArpTable.Add(new ArpTableEntry
                    {
                        IpAddress = arp.SourceIp,
                        MacAddress = arp.SourceMac
                    });

                    RuntimeState.PendingArpRequests.Remove(arp.SourceIp);

                    var pending = RuntimeState.PendingPackets
                                              .Where(p => p.NextHopIp == arp.SourceIp)
                                              .ToList();

                    RuntimeState.PendingPackets = new Queue<PendingPacket>(
                            RuntimeState.PendingPackets.Where(p => p.NextHopIp != arp.SourceIp)
                    );

                    foreach (var pendingPacket in pending)
                    {
                        var outInterface = NetworkConfig.Interfaces
                            .First(i => i.Id == pendingPacket.InterfaceId);

                        pendingPacket.Packet.SourceMac = outInterface.MacAddress;
                        pendingPacket.Packet.DestinationMac = arp.SourceMac;

                        var nextDevicee = outInterface.PhysicalPort.ConnectedTo.Owner;

                        yield return new SimmulationEvent
                        {
                            Packet = pendingPacket.Packet,

                            FromDeviceId = this.Id,
                            ToDeviceId = nextDevice.Id,

                            FromPortId = outInterface.PhysicalPort.Id,
                            ToPortId = outInterface.PhysicalPort.ConnectedTo.Id
                        };
                    }

                    yield break;
                }
            }
        }
        private IEnumerable<SimmulationEvent> HandleIcmp(IcmpPacket icmp, RouterNetworkInterface inIface)
        {
            if (icmp.DestinationIp == inIface.IpV4Address)
            {
                yield break;
            }

            var routes = RuntimeState.StaticRoutingTable
                .Select(r => new
                {
                    r.DestinationNetwork,
                    r.SubnetMask,
                    r.InterfaceId,
                    r.NextHopIp
                })
                .Concat(RuntimeState.AutomaticRoutingTable.Select(r => new
                {
                    DestinationNetwork = r.Network,
                    r.SubnetMask,
                    r.InterfaceId,
                    NextHopIp = (string)null
                }));

            var route = routes.FirstOrDefault(r =>
                IpUtils.IsSameSubnet(icmp.DestinationIp, r.DestinationNetwork, r.SubnetMask));

            if (route == null)
                yield break;

            var outIface = NetworkConfig.Interfaces
                .FirstOrDefault(i => i.Id == route.InterfaceId);

            if (outIface == null || !outIface.IsEnabled)
                yield break;

            var nextHopIp = route.NextHopIp ?? icmp.DestinationIp;

            var arp = RuntimeState.ArpTable
                .FirstOrDefault(a => a.IpAddress == nextHopIp);

            if (arp == null)
            {
                if (RuntimeState.PendingArpRequests.Add(nextHopIp))
                {
                    var arpRequest = new ArpPacket
                    {
                        ArpType = ArpType.Request,
                        SourceIp = outIface.IpV4Address,
                        SourceMac = outIface.MacAddress,
                        DestinationIp = nextHopIp,
                        DestinationMac = "FF:FF:FF:FF:FF:FF"
                    };

                    var nextDevice = outIface.PhysicalPort.ConnectedTo.Owner;

                    yield return new SimmulationEvent
                    {
                        Packet = arpRequest,
                        FromDeviceId = this.Id,
                        ToDeviceId = nextDevice.Id,
                        FromPortId = outIface.PhysicalPort.Id,
                        ToPortId = outIface.PhysicalPort.ConnectedTo.Id
                    };
                }

                RuntimeState.PendingPackets.Enqueue(new PendingPacket
                {
                    Packet = icmp,
                    NextHopIp = nextHopIp,
                    InterfaceId = outIface.Id
                });

                yield break;
            }

            icmp.SourceMac = outIface.MacAddress;
            icmp.DestinationMac = arp.MacAddress;

            var nextDeviceFinal = outIface.PhysicalPort.ConnectedTo.Owner;

            yield return new SimmulationEvent
            {
                Packet = icmp,
                FromDeviceId = this.Id,
                ToDeviceId = nextDeviceFinal.Id,
                FromPortId = outIface.PhysicalPort.Id,
                ToPortId = outIface.PhysicalPort.ConnectedTo.Id
            };
        }

        public override IEnumerable<INetworkInterface> GetInterfaces() => NetworkConfig.Interfaces;
        public override IEnumerable<Port> GetPorts() => Ports;
    }
}
