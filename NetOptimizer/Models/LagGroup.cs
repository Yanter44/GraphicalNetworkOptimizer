using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class LagGroup
    {
        public int Id { get; set; }

        public string Name { get; set; } 

        public LagMode Mode { get; set; }

        //public LagLoadBalanceMode LoadBalance { get; set; }

        //public List<LagMember> Members { get; set; } = new();
    }
}
