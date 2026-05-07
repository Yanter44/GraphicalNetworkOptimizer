using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels.Switch
{
    public class NetworkInterfaceViewModel : INotifyPropertyChanged
    {
        private readonly SwitchNetworkInterface _model;
        public NetworkInterfaceViewModel(SwitchNetworkInterface model)
        {
            _model = model;
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

        public SwitchPortMode SwitchPortMode
        {
            get => _model.SwitchPortMode;
            set
            {
                if (_model.SwitchPortMode == value) return;

                _model.SwitchPortMode = value;
                OnPropertyChanged();

                if (value == SwitchPortMode.Trunk)
                {
                    _model.AccessVlan = null;
                }
                else
                {
                    _model.AllowedVlans = null;
                }

                OnPropertyChanged(nameof(AccessVlan));
                OnPropertyChanged(nameof(AllowedVlans));
            }
        }

        public int? AccessVlan
        {
            get => _model.AccessVlan;
            set
            {
                if (_model.AccessVlan == value) return;
                _model.AccessVlan = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int>? AllowedVlans
        {
            get => _model.AllowedVlans == null
                ? null
                : new ObservableCollection<int>(_model.AllowedVlans);

            set
            {
                _model.AllowedVlans = value?.ToList();
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
