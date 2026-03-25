using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace NetOptimizer.Models.Dtos
{
    public class AvailableDevicesForEditorDto : INotifyPropertyChanged
    {
        public string DeviceName { get; set; }
        public DeviceType Type { get; set; }
        private int _count;
        public int MaxCount { get; set; }
        public bool HasCount { get; set; } = true;
        public int Count { get => _count; set { if (_count != value) { _count = value; OnPropertyChanged(); } } }
        public string ImagePath => Type switch
        {
            DeviceType.Router => "Assets/Images/router.png",
            DeviceType.Switch => "Assets/Images/switch.png",
            DeviceType.PC => "Assets/Images/pc.png",
            DeviceType.IpVideoCam => "Assets/Images/videocam.png",
            DeviceType.Server => "Assets/Images/server.png",
            DeviceType.AccessPoint => "Assets/Images/accesspoint.png",
            _ => "Assets/Images/delete.png"
        };
        private object _deviceSubMenuViewModel;
        public object DeviceSubMenuViewModel
        {
            get => _deviceSubMenuViewModel;
            init => _deviceSubMenuViewModel = value;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
