using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Helpers;
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
        public decimal AveragePrice { get; set; }
        public PcDevice(string name, PcSettings settings) : base(name)
        {
            this.Type = DeviceType.PC;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.HardwareSpecs = settings.HardwareSpecs;
            this.WifiOptions = settings.WifiOptions;
            this.AveragePrice = settings.AveragePrice;
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
                interfacee.IsEnabled = true;
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
            var iface = NetworkConfig.Interfaces[0];

            if (iface == null || !iface.IsEnabled)
                yield break;

            if (iface.PhysicalPort?.ConnectedTo == null)
                yield break;

            if (iface.IpV4Address == "0.0.0.0")
                yield break;

            // 🧠 1. ПАКЕТ ДЛЯ МЕНЯ
            if (packet.DestinationIp == iface.IpV4Address)
            {
                    if (packet.Type == PacketType.ICMP)
                    {
                        var replyPacket = new Packet
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceDeviceId = this.Id,
                            SourceIp = iface.IpV4Address,
                            DestinationIp = packet.SourceIp,
                            Type = PacketType.ICMP,
                            TTL = 20
                        };

                        var nexttDevice = iface.PhysicalPort.ConnectedTo.Owner;

                        if (nexttDevice == null)
                            yield break;

                        yield return new SimmulationEvent
                        {
                            PacketId = replyPacket.Id,
                            FromDeviceId = this.Id,
                            ToDeviceId = nexttDevice.Id,
                            FromInterfaceId = iface.Name,
                            PacketType = PacketType.ICMP
                        };
                    }
                    yield break;              
            }

            // 🧠 2. ПАКЕТ НЕ ДЛЯ МЕНЯ → forward
            var nextDevice = iface.PhysicalPort.ConnectedTo.Owner;

            if (nextDevice == null)
                yield break;

            bool sameSubnet = IpUtils.IsSameSubnet(
                iface.IpV4Address,
                packet.DestinationIp,
                iface.SubnetMask
            );

            string nextHopIp;

            if (sameSubnet)
            {
                nextHopIp = packet.DestinationIp;
            }
            else
            {
                if (iface.DefaultGateway == "0.0.0.0")
                    yield break;

                nextHopIp = iface.DefaultGateway;
            }

            yield return new SimmulationEvent
            {
                PacketId = packet.Id,
                FromDeviceId = this.Id,
                ToDeviceId = nextDevice.Id,
                FromInterfaceId = iface.Name,
                PacketType = packet.Type
            };
        }
            

    }
}
