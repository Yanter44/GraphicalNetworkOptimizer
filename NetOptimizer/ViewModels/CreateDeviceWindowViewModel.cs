using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NetOptimizer.ViewModels
{
    public class CreateDeviceWindowViewModel : INotifyPropertyChanged
    {
        private DeviceToAddDto _creatableDevice;
        public DeviceToAddDto CreatableDevice { get => _creatableDevice; set { _creatableDevice = value; OnPropertyChanged(); } }

        private DeviceSettingsBase _deviceSettings;
        public DeviceSettingsBase DeviceSettings
        {
            get => _deviceSettings;
            set { _deviceSettings = value; OnPropertyChanged(); }
        }
        public ICommand CloseCommand { get; }
        public ICommand ValidateDeviceAndAddToCanvasCommand { get; }
        public CreateDeviceWindowViewModel(DeviceToAddDto device)
        {
            CreatableDevice = device;
            CloseCommand = new RelayCommand(obj => { if (obj is Window window) { window.Close(); } });
            ValidateDeviceAndAddToCanvasCommand = new RelayCommand(ValidateDeviceAndAddToCanvas);
            InitializeDeviceSettings();
        }
        private void InitializeDeviceSettings()
        {
            if (CreatableDevice == null) return;

            switch (CreatableDevice.Type)
            {
                case DeviceType.PC:
                    DeviceSettings = new PcSettingModel();
                    break;

                case DeviceType.Printer:
                    DeviceSettings = new PrinterSettingModel();
                    break;
                case DeviceType.Switch:
                    DeviceSettings = new SwitchSettingModel();
                    break;

                default:
                    DeviceSettings = null;
                    break;
            }
        }
        private void ValidateDeviceAndAddToCanvas()
        {
            var currentSettings = DeviceSettings;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
