using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels.PC
{
    public class PcNetworkInterfaceViewModel : INotifyPropertyChanged
    {
        private readonly PcNetworkInterface _model;
        public bool IsEnabled
        {
            get => _model.IsEnabled;
            set
            {
                if (_model.IsEnabled == value) return;
                _model.IsEnabled = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name == value) return;
                _model.Name = value;
                OnPropertyChanged();
            }
        }
        
        public string IpV4Address
        {
            get => _model.IpV4Address;
            set
            {
                if (_model.IpV4Address == value) return;
                _model.IpV4Address = value;
                OnPropertyChanged();
            }
        }
        public string SubnetMask
        {
            get => _model.SubnetMask;
            set
            {
                if(_model.SubnetMask == value) return;
                _model.SubnetMask = value;
                OnPropertyChanged();
            }
        }
        public string DefaultGateway 
        {
            get => _model.DefaultGateway;
            set
            {
                if(_model.DefaultGateway  == value) return;
                _model.DefaultGateway = value;
                OnPropertyChanged();
            }
        }
        public string DNS
        {
            get => _model.DNS;
            set
            {
                if (_model.DNS == value) return;
                _model.DNS = value;
                OnPropertyChanged();
            }
        }
        public string MacAddress 
        {
            get => _model.MacAddress;
            set
            {
                if(_model.MacAddress == value) return;
                _model.MacAddress = value;
                OnPropertyChanged();
            } 
        }

        public PcNetworkInterfaceViewModel(PcNetworkInterface model)
        {
            _model = model;
        }
  
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
