using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace NetOptimizer.Models.UIElements
{
    public class DeviceOnCanvas : INotifyPropertyChanged
    {
        private double _x;
        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

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
        public BitmapImage ImageSource
        {
            get
            {
                var uri = new Uri($"pack://application:,,,/{ImagePath}", UriKind.Absolute);
                return new BitmapImage(uri);
            }
        }
        public DeviceOnCanvas(Device logicDevice, double x = 0, double y = 0)
        {
            LogicDevice = logicDevice;
            _x = x;
            _y = y;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
