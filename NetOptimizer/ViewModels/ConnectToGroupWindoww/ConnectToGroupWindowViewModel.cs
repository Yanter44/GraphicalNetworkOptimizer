using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Views.DopViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.ConnectToGroupWindoww
{
    public class ConnectToGroupWindowViewModel : INotifyPropertyChanged
    {

        private readonly IWindowNavigator _windowNavigator;
        private DeviceGroup _selectedGroup;
        public DeviceGroup SelectedGroup { get => _selectedGroup; set { _selectedGroup = value; OnPropertyChanged(); } }

        public event Action RequestClose;
        public event Func<(DeviceGroup, AvailableDevicesForEditorDto), bool> DeviceConnected;
        public AvailableDevicesForEditorDto TriedConnectDevice { get; set; }
        public ObservableCollection<DeviceGroup> DeviceGroups { get; set; }    
        public ICommand CloseCommand { get; }
        public ICommand ConnectDeviceToGroupCommand { get;}

        public ConnectToGroupWindowViewModel(IWindowNavigator windowNavigator)
        {
            _windowNavigator = windowNavigator;
            CloseCommand = new RelayCommand(CloseWindow);
            ConnectDeviceToGroupCommand = new RelayCommand(ConnectDeviceToGroup);
        }
        private void ConnectDeviceToGroup()
        {
            if (SelectedGroup != null)
            {
                var result = DeviceConnected.Invoke((SelectedGroup, TriedConnectDevice));
                if (result) 
                   CloseWindow();
                else { _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>("Произошла ошибка при попытке "); }
            }
            else
            {
                _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>("Группа не выбрана!");
            }
        }
        private void CloseWindow()
        {
            RequestClose?.Invoke();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
