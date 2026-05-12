using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface INetworkInterface
    {
        string Id { get; }
        string Name { get; }
        bool IsEnabled { get; }
        Port PhysicalPort { get; }
    }
}
