using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.DeviceModels
{
    public class DeviceGroup : INotifyPropertyChanged
    {
        private string _groupName;
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value; OnPropertyChanged(); }
        }
        public ObservableCollection<AvailableDevicesForEditorDto> GroupDevices { get; set; } = new ObservableCollection<AvailableDevicesForEditorDto>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
