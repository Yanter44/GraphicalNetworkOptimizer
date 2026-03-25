
using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels;

namespace NetOptimizer.Models
{
    public class Port
    {
        public string PortName { get; set; }
        public string PortNumber { get; set; }
        public PortType Type { get; set; }
        public Device Owner { get; set; }
        public Port ConnectedTo { get; set; }
        public bool IsLinked => ConnectedTo != null;
        public bool IsInitiator { get; set; }
        public void LinkTo(Port remotePort)
        {
            if (this.IsLinked || remotePort.IsLinked)
                throw new Exception("Один из портов уже занят!");

            this.ConnectedTo = remotePort;
            remotePort.ConnectedTo = this;

            this.IsInitiator = true;
            remotePort.IsInitiator = false;
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
