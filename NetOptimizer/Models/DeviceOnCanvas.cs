using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models
{
    public class DeviceOnCanvas : INotifyPropertyChanged
    {

        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => _isSelected = value; }
        public double X { get; set; }
        public double Y { get; set; }

        public Device LogicDevice { get; init; }
        public string DeviceName => LogicDevice.Name;
        public string DeviceModel => LogicDevice.DeviceModel;
        public string ImagePath => LogicDevice.Type switch
        {
            DeviceType.Router => "Assets/Images/router.png",
            DeviceType.Switch => "Assets/Images/switch.png",
            DeviceType.PC => "Assets/Images/pc.png",
            _ => "Assets/Images/delete.png"
        };
        public DeviceOnCanvas(Device logicDevice, double x = 0, double y = 0)
        {
            LogicDevice = logicDevice;
            X = x;
            Y = y;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
