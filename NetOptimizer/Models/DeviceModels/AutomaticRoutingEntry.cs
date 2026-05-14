using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class AutomaticRoutingEntry
    {
        public string Network { get; set; }        
        public string SubnetMask { get; set; }   
        public string InterfaceId { get; set; }  
    }
}
