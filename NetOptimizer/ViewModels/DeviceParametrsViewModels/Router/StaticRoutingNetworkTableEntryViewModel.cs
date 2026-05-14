using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels.Router
{
    public class StaticRoutingNetworkTableEntryViewModel : INotifyPropertyChanged
    {
        private string _destinationNetwork;
        public string DestinationNetwork
        {
            get => _destinationNetwork;
            set
            {
                if (_destinationNetwork == value) return;
                _destinationNetwork = value;
                OnPropertyChanged();
            }
        }

        private string _subnetMask;
        public string SubnetMask
        {
            get => _subnetMask;
            set
            {
                if (_subnetMask == value) return;
                _subnetMask = value;
                OnPropertyChanged();
            }
        }

        private string _nextHopIp;
        public string NextHopIp
        {
            get => _nextHopIp;
            set
            {
                if (_nextHopIp == value) return;
                _nextHopIp = value;
                OnPropertyChanged();
            }
        }
        private string _interfaceName;
        public string InterfaceName
        {
            get => _interfaceName;
            set
            {
                if(_interfaceName == value) return;
                _interfaceName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
