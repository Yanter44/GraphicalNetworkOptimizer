using NetOptimizer.Models.DeviceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class NetworkMap
    {
        public string NetworkName { get; set; }
        public CanvasSettings CanvasSettings { get; set;}
        public List<Device> Devices { get; set; }
    }
}
