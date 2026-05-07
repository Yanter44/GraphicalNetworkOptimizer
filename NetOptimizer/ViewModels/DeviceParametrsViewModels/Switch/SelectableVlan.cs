using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels.Switch
{
    public class SelectableVlan : INotifyPropertyChanged
    {
        private int _vlanId;
        private string _vlanName;
        private bool _isSelected;

        public int VlanId { get => _vlanId; set { if (_vlanId == value) return; _vlanId = value; OnPropertyChanged(); } }
        public string VlanName { get => _vlanName; set {if (_vlanName == value) return; _vlanName = value; OnPropertyChanged(); } }
        public bool IsSelected { get => _isSelected; set{ if (_isSelected == value) return; _isSelected = value; OnPropertyChanged();}}

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
