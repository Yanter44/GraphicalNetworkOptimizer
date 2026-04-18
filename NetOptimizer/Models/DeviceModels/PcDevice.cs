using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.NetworkSettings;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
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
        public PcNetworkSettings NetworkSettings { get; set; }
        public decimal AveragePrice { get; set; }
        public PcDevice(string name, PcSettings settings) : base(name)
        {
            this.Type = DeviceType.PC;
            this.Vendor = settings.Vendor;
            this.DeviceModel = settings.Model;
            this.HardwareSpecs = settings.HardwareSpecs;
            this.WifiOptions = settings.WifiOptions;
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
