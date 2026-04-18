using NetOptimizer.Common;
using NetOptimizer.Models.UIElements;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace NetOptimizer.ViewModels.DeviceParametrs
{
    public class DeviceParametrsViewModel : INotifyPropertyChanged
    {
        public event Action RequestClose;

        private DeviceOnCanvas _openedDevice;
        public DeviceOnCanvas OpenedDevice { get => _openedDevice; set { _openedDevice = value; OnPropertyChanged(); } }
        public ICommand CloseCommand { get; }
        public DeviceParametrsViewModel(DeviceOnCanvas device)
        {
            OpenedDevice = device;
            CloseCommand = new RelayCommand(obj => { if (obj is Window window) { window.Close(); } });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
