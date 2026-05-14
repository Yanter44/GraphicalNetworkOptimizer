using MediatR;
using NetOptimizer.MediatR.Notifications;
using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels.Router
{
    public class RouterNetworkInterfaceViewModel : INotifyPropertyChanged
    {
        private readonly RouterNetworkInterface _model;
        private readonly IMediator _mediator;
        public RouterNetworkInterfaceViewModel(RouterNetworkInterface model, IMediator mediator)
        {
            _model = model;
            _mediator = mediator;
        }
        public bool IsEnabled
        {
            get => _model.IsEnabled;
            set
            {
                if (_model.IsEnabled == value) return;
                _model.IsEnabled = value;
                OnPropertyChanged();
                _ = _mediator.Publish(new InterfaceStateChangedNotification
                {
                    Port = _model.PhysicalPort,
                    IsUp = value
                });

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
                if (_model.SubnetMask == value) return;
                _model.SubnetMask = value;
                OnPropertyChanged();
            }
        }
        public string MacAddress
        {
            get => _model.MacAddress;
            set
            {
                if (_model.MacAddress == value) return;
                _model.MacAddress = value;
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
