using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww;
using NetOptimizer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public class SwitchDeviceViewModel : DeviceViewModelBase
    {
        private readonly IWindowNavigator _windownavigator;
        public ICommand OpenAddVlanCommand { get; }

        public SwitchDeviceViewModel(DeviceOnCanvas device, IWindowNavigator windownavigator) : base(device)
        {
            _windownavigator = windownavigator;
            OpenAddVlanCommand = new RelayCommand(OpenAddVlan);
        }

        private void OpenAddVlan()
        {
            _windownavigator.ShowModalView<CreateVlanOnDeviceWindow, CreateVlanOnDeviceViewModel>(Device.LogicDevice);
        }
    }
}
