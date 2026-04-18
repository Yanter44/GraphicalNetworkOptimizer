using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;
using NetOptimizer.ViewModels.CreateDeviceWindoww;
using NetOptimizer.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.MainWindow
{
    public class SandboxViewModel : INotifyPropertyChanged
    {
        public Action<UIToolElementToAddDto> StartDrawTool;
        public ObservableCollection<DeviceToAddDto> AvailableDevices { get; } = new ObservableCollection<DeviceToAddDto>();
        public ObservableCollection<UIToolElementToAddDto> AvailableTools { get; } = new ObservableCollection<UIToolElementToAddDto>();
        private UIToolElementToAddDto _selectedTool;
        public ICommand OpenCreateDeviceWindowCommand { get; }
        public ICommand PrepareForDrawSelectedUIElementCommand { get; }
        private readonly IWindowNavigator _windowNavigator;
        public SandboxViewModel(IWindowNavigator windowNavigator)
        {
            _windowNavigator = windowNavigator;
            OpenCreateDeviceWindowCommand = new RelayCommand(p => OpenCreateDeviceWindow((DeviceToAddDto)p));
            PrepareForDrawSelectedUIElementCommand = new RelayCommand(uielem => PrepareForDrawSelectedUIElement((UIToolElementToAddDto)uielem));     
        }
        public void PrepareForDrawSelectedUIElement(UIToolElementToAddDto uiElement)
        {
            if (_selectedTool != null)
                _selectedTool.IsSelected = false;

            _selectedTool = uiElement;
            _selectedTool.IsSelected = true;

            StartDrawTool?.Invoke(uiElement);
        }
        public void OpenCreateDeviceWindow(DeviceToAddDto device) 
        {
            _windowNavigator.ShowModalView<Views.CreateDeviceWindow, CreateDeviceWindowViewModel>(device);  
        }
        public void InitializeAllAvailableTools()
        {
            var types = Enum.GetValues(typeof(UIToolElementType));

            foreach (UIToolElementType type in types)
            {
                AvailableTools.Add(new UIToolElementToAddDto()
                {
                    ToolName = type.ToString(),
                    Type = type,
                });
            }
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
