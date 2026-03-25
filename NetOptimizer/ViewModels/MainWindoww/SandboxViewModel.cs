using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.Dtos;
using NetOptimizer.ViewModels.CreateDeviceWindow;
using NetOptimizer.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.MainWindow
{
    public class SandboxViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DeviceToAddDto> AvailableDevices { get; } = new ObservableCollection<DeviceToAddDto>();
        public ICommand OpenCreateDeviceWindowCommand { get; }
        private readonly IWindowNavigator _windowNavigator;
        public SandboxViewModel(IWindowNavigator windowNavigator)
        {
            _windowNavigator = windowNavigator;
            OpenCreateDeviceWindowCommand = new RelayCommand(p => OpenCreateDeviceWindow((DeviceToAddDto)p));
        }
        public void OpenCreateDeviceWindow(DeviceToAddDto device) 
        {
            _windowNavigator.ShowModalView<Views.CreateDeviceWindow, CreateDeviceWindowViewModel>(device);  
        }
        public void InitializeAllAvailableDevices()
        {
            var types = Enum.GetValues(typeof(DeviceType));

            foreach (DeviceType type in types)
            {
                AvailableDevices.Add(new DeviceToAddDto()
                {
                    DeviceName = type.ToString(),
                    Type = type
                });
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
