using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels.Router
{
    public class AutomaticRoutingNetworkTableViewModel : INotifyPropertyChanged
    {
        private string _network;
        public string Network
        {
            get => _network;
            set
            {
                if (_network == value) return;
                _network = value;
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

        private string _interfaceName;
        public string InterfaceName
        {
            get => _interfaceName;
            set
            {
                if (_interfaceName == value) return;
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
