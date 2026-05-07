using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class MacTableEntry
    {
        public int VlanId { get; set; }
        public string MacAddress { get; set; }
        public MacEntryType Type { get; set; }
        public string InterfaceName { get; set; }
    }
}
