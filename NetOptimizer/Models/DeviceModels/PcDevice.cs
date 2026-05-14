using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Helpers;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.NetworkSettings;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class PcDevice : Device
    {
        public string Vendor { get; set; }
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
        public PcNetworkConfiguration NetworkConfig { get; set; }
        public PcRuntimeState RuntimeState { get; set; }
        public decimal AveragePrice { get; set; }
        public PcDevice(string name, PcSettings settings) : base(name)
        {
            this.Type = DeviceType.PC;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.HardwareSpecs = settings.HardwareSpecs;
            this.WifiOptions = settings.WifiOptions;
            this.AveragePrice = settings.AveragePrice;
            this.RuntimeState = new PcRuntimeState();
            this.NetworkConfig = new PcNetworkConfiguration
            {
                Hostname = name,
                Interfaces = new List<PcNetworkInterface>()
            };
            GeneratePorts(settings.Ports);
            ConfigureInterface();
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

                    var prefix = GetInterfacePrefix(dto.Speed, dto.Type);
                    var portId = $"{slot}/{port}";

                    var portEntity = new Port
                    {
                        PortName = $"{dto.Type}",
                        PortNumber = $"{slot}/{port}",
                        Type = dto.Type,
                        Owner = this
                    };

                    Ports.Add(portEntity);
                    this.NetworkConfig.Interfaces.Add(new PcNetworkInterface
                    {
                        Name = $"{prefix}{portId}",
                        PhysicalPort = portEntity
                    });
                }
            }
        }
        private void ConfigureInterface()
        {
            foreach (var interfacee in NetworkConfig.Interfaces)
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
            PcNetworkInterface iface;
            if (string.IsNullOrEmpty(InPortId))
            {
                iface = NetworkConfig.Interfaces.FirstOrDefault(i => i.IsEnabled);
            }
            else
            {
                iface = NetworkConfig.Interfaces.FirstOrDefault(i => i.PhysicalPort.Id == InPortId);
            }

            if (iface == null || !iface.IsEnabled)
                yield break;

            if (iface.PhysicalPort?.ConnectedTo == null)
                yield break;

            if (packet.TTL <= 0)
                yield break;

            if (packet is ArpPacket arp)
            {
                foreach (var ev in HandleArp(arp, iface))
                    yield return ev;

                yield break;
            }

            if (packet is IcmpPacket icmp)
            {
                foreach (var ev in HandleIcmp(icmp, iface))
                    yield return ev;

                yield break;
            }

            foreach (var ev in HandleForward(packet, iface))
                yield return ev;
        }
        private IEnumerable<SimmulationEvent> HandleArp(ArpPacket arp, PcNetworkInterface iface)
        {
            var nextDevice = iface.PhysicalPort.ConnectedTo.Owner;

            if (nextDevice == null)
                yield break;

            if (arp.ArpType == ArpType.Request)
            {
                if (arp.DestinationIp != iface.IpV4Address)
                    yield break;

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

                yield break;
            }
            if (arp.ArpType == ArpType.Response)
            {
                RuntimeState.ArpTable.Add(new ArpTableEntry
                {
                    IpAddress = arp.SourceIp,
                    MacAddress = arp.SourceMac
                });

                RuntimeState.PendingArpRequests.Remove(arp.SourceIp);

                var pending = RuntimeState.PendingPackets.ToList();
                RuntimeState.PendingPackets.Clear();

                foreach (var pendingPacket in pending)
                {
                    var macEntry = RuntimeState.ArpTable
                        .FirstOrDefault(x => x.IpAddress == pendingPacket.NextHopIp);

                    if (macEntry == null)
                        continue;

                    pendingPacket.Packet.SourceMac = iface.MacAddress;
                    pendingPacket.Packet.DestinationMac = macEntry.MacAddress;

                    yield return new SimmulationEvent
                    {
                        Packet = pendingPacket.Packet,

                        FromDeviceId = this.Id,
                        ToDeviceId = iface.PhysicalPort.ConnectedTo.Owner.Id,

                        FromPortId = iface.PhysicalPort.Id,
                        ToPortId = iface.PhysicalPort.ConnectedTo.Id
                    };
                }
                yield break;
            }
        }
        private IEnumerable<SimmulationEvent> HandleIcmp(IcmpPacket icmp, PcNetworkInterface iface)
        {
            var nextDevice = iface.PhysicalPort.ConnectedTo.Owner;

            if (icmp.DestinationIp == iface.IpV4Address)
            {
                if (icmp.IcmpType == IcmpType.EchoRequest)
                {
                    var reply = new IcmpPacket
                    {
                        SourceIp = iface.IpV4Address,
                        DestinationIp = icmp.SourceIp,
                        IcmpType = IcmpType.EchoReply,
                        Sequence = icmp.Sequence
                    };
                    var arpEntry = RuntimeState.ArpTable.FirstOrDefault(x => x.IpAddress == icmp.SourceIp);

                    if (arpEntry != null)
                    {
                        reply.SourceMac = iface.MacAddress;
                        reply.DestinationMac = arpEntry.MacAddress;
                    }
                    reply.SourceMac = iface.MacAddress;
                    reply.DestinationMac = icmp.SourceMac;
                    yield return new SimmulationEvent
                    {
                        Packet = reply,
                        FromDeviceId = this.Id,
                        ToDeviceId = nextDevice.Id,

                        FromPortId = iface.PhysicalPort.Id,
                        ToPortId = iface.PhysicalPort.ConnectedTo.Id
                    };
                }

                yield break;
            }

            var nextHopIp = IpUtils.IsSameSubnet(iface.IpV4Address, icmp.DestinationIp, iface.SubnetMask) ? icmp.DestinationIp : iface.DefaultGateway;

            var arp = RuntimeState.ArpTable.FirstOrDefault(x => x.IpAddress == nextHopIp);
            if (arp == null)
            {
                if (RuntimeState.PendingArpRequests.Add(nextHopIp))
                {
                    var arpRequest = new ArpPacket
                    {
                        SourceMac = iface.MacAddress,
                        DestinationMac = "FF:FF:FF:FF:FF:FF",
                        ArpType = ArpType.Request,
                        SourceIp = iface.IpV4Address,
                        DestinationIp = nextHopIp
                    };

                    yield return new SimmulationEvent
                    {
                        Packet = arpRequest,
                        FromDeviceId = this.Id,
                        ToDeviceId = nextDevice.Id,

                        FromPortId = iface.PhysicalPort.Id,
                        ToPortId = iface.PhysicalPort.ConnectedTo.Id
                    };
                }
                RuntimeState.PendingPackets.Enqueue(new PendingPacket
                {
                    Packet = icmp,
                    NextHopIp = nextHopIp,
                });
                yield break;
            }
            icmp.SourceMac = iface.MacAddress;
            icmp.DestinationMac = arp.MacAddress;
            yield return new SimmulationEvent
            {
                Packet = icmp,
                FromDeviceId = this.Id,
                ToDeviceId = nextDevice.Id,

                FromPortId = iface.PhysicalPort.Id,
                ToPortId = iface.PhysicalPort.ConnectedTo.Id
            };
        }
        private IEnumerable<SimmulationEvent> HandleForward(Packet packet, PcNetworkInterface iface)
        {
            var nextDevice = iface.PhysicalPort.ConnectedTo.Owner;

            if (nextDevice == null)
                yield break;

            packet.TTL--;

            yield return new SimmulationEvent
            {
                Packet = packet,
                FromDeviceId = this.Id,
                ToDeviceId = nextDevice.Id,

                FromPortId = iface.PhysicalPort.Id,
                ToPortId = iface.PhysicalPort.ConnectedTo.Id
            };
        }

        public override IEnumerable<INetworkInterface> GetInterfaces() => NetworkConfig.Interfaces;

        public override IEnumerable<Port> GetPorts() => Ports;
    }
}
