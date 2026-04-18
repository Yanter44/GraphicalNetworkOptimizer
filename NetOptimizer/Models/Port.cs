
using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels;
using System.ComponentModel;

namespace NetOptimizer.Models
{
    public class Port : INotifyPropertyChanged
    {
        public string PortName { get; set; }
        public string PortNumber { get; set; }
        public PortType Type { get; set; }
        public Device Owner { get; set; }

        private Port _connectedTo;
        public Port ConnectedTo
        {
            get => _connectedTo;
            set
            {
                if (_connectedTo != value)
                {
                    _connectedTo = value;

                    OnPropertyChanged(nameof(ConnectedTo));
                    OnPropertyChanged(nameof(IsLinked)); 
                }
            }
        }
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
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
