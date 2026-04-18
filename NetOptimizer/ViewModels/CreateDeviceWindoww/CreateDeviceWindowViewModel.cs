using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Services;
using NetOptimizer.ViewModels.DeviceSettingss;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.CreateDeviceWindoww
{
    public class CreateDeviceWindowViewModel : INotifyPropertyChanged
    {
        public event Action RequestClose;
        private DeviceToAddDto _creatableDevice;
        public DeviceToAddDto CreatableDevice { get => _creatableDevice; set { _creatableDevice = value; OnPropertyChanged(); } }

        private DeviceSettingsBase _deviceSettings;
        public DeviceSettingsBase DeviceSettings
        {
            get => _deviceSettings;
            set { _deviceSettings = value; OnPropertyChanged(); }
        }
        private readonly DeviceCatalogService _catalogService;
        public ICommand CloseCommand { get; }
        public ICommand ValidateDeviceAndAddToCanvasCommand { get; }
        public CreateDeviceWindowViewModel(DeviceToAddDto device, DeviceCatalogService catalogService)
        {
            _catalogService = catalogService;
            CreatableDevice = device;
            CloseCommand = new RelayCommand(obj => { if (obj is Window window) { window.Close(); } });
            ValidateDeviceAndAddToCanvasCommand = new RelayCommand(ValidateDeviceAndAddToCanvas);
            InitializeDeviceSettings();
        }
        private void InitializeDeviceSettings()
        {
            if (CreatableDevice == null) return;

            DeviceSettings = CreatableDevice.Type switch
            {
                DeviceType.PC => new PcSettingsViewModel(_catalogService.AvailablePcs),
                DeviceType.Switch => new SwitchSettingViewModel(_catalogService.AvailableSwitches),
                DeviceType.Router => new RouterSettingsViewModel(_catalogService.AvailableRouters),
                _ => null
            };
        }
        private void ValidateDeviceAndAddToCanvas()
        {
            if (CreatableDevice == null || DeviceSettings == null) return;

            EventAggregator.Instance.PublishDeviceCreated(CreatableDevice, DeviceSettings);
            RequestClose?.Invoke();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
