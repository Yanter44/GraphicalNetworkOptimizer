using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface ILayer3Interface : INetworkInterface
    { 
        string IpV4Address { get; }
        string SubnetMask { get; } 
    }
}
