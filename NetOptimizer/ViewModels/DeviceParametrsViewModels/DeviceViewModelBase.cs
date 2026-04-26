using NetOptimizer.Models.UIElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public abstract class DeviceViewModelBase : INotifyPropertyChanged
    {
        public DeviceOnCanvas Device { get; }
        protected DeviceViewModelBase(DeviceOnCanvas device)
        {
            Device = device;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
