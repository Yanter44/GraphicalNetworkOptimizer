using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class RawLink
    {
        public string Source { get; set; }      
        public string Target { get; set; }    
        public string SourcePort { get; set; } 
        public string TargetPort { get; set; }
    }
}
