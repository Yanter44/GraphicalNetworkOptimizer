using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public abstract class Device
    {
        public string Name { get; private set; }
        public string DeviceModel { get; set; }
        public DeviceType Type { get; protected set; }
        public List<EthernetPort> Ports { get; private set; } = new List<EthernetPort>();

        public Device(string name,int portCount)
        {
            this.Name = name;
            for (int i = 0; i < portCount; i++)
            {
                Ports.Add(new EthernetPort { PortName = "EthernetPort", PortNumber = GetPortName(i), Owner = this });
            }
        }
        public string GetPortName(int portIndex)
        {
            int portsPerModule = 4; 

            int module = portIndex / portsPerModule; 
            int port = portIndex % portsPerModule;   

            return $"{module}/{port}";
        }
    }
}
