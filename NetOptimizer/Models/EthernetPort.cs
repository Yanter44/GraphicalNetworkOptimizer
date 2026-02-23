using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models
{
    public class EthernetPort
    {
        public string PortName { get; set; }
        public string PortNumber { get; set; }
        public Device Owner { get; set; }
        public EthernetPort ConnectedTo { get; set; }
        public bool IsLinked => ConnectedTo != null;
        public void LinkTo(EthernetPort remotePort)
        {
            if (this.IsLinked || remotePort.IsLinked)
                throw new Exception("Один из портов уже занят!");

            this.ConnectedTo = remotePort;
            remotePort.ConnectedTo = this;
        }

        public void Unlink()
        {
            if (ConnectedTo != null)
            {
                var remote = ConnectedTo;
                this.ConnectedTo = null;
                remote.ConnectedTo = null; 
            }
        }
    }
}
