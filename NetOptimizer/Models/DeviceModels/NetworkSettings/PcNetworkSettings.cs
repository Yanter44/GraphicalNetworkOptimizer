using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.DeviceModels.NetworkSettings
{
    public class PcNetworkSettings : INotifyPropertyChanged
    {
        private string _ipAddress = "";
        private string _subnetMask = "";
        private string _gateway = "";
        private string _dns = "";
        private bool _isDhcp = false;
        public string IpAddress { get => _ipAddress; set { _ipAddress = value; OnPropertyChanged(); } }
        public string SubnetMask { get => _subnetMask; set { _subnetMask = value; OnPropertyChanged(); }}
        public string Gateway { get => _gateway; set { _gateway = value; OnPropertyChanged(); }}
        public string DNS { get => _dns; set { _dns = value; OnPropertyChanged(); }}
        public bool IsDhcp { get => _isDhcp; set { _isDhcp = value; OnPropertyChanged(); }}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
