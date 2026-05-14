using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.Dtos
{
    public class RouteDto
    {
        public string Network { get; set; }
        public string SubnetMask { get; set; }
        public string NextHopIp { get; set; }
    }
}
