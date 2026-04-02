using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class DeviceAllocation : INotifyPropertyChanged
    {
        public DeviceGroup Group { get; set; } 
        public AvailableDevicesForEditorDto Device { get; set; } 

        private int _count;
        public int Count
        {
            get => _count;
            set { if (_count != value) { _count = value; OnPropertyChanged(); } }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
