using MediatR;
using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.AddRouteEntryToRoutingTableWindoww;
using NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.PC;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.Router;
using NetOptimizer.Views;
using NetOptimizer.Views.AddNewEntryToRoutingTableWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public class RouterDeviceViewModel : DeviceViewModelBase
    {
        private readonly IWindowNavigator _windownavigator;
        private RouterDevice RouterDevice;
        public ObservableCollection<RouterNetworkInterfaceViewModel> RouterInterfaceViewModel { get; set; } = new();
        public ObservableCollection<ArpTableEntryViewModel> ArpTableViewModel { get; set; } = new();
        public ObservableCollection<RoutingNetworkTableEntryViewModel> RoutingNetworkTableViewModel { get; set; } = new();
        public ICommand OpenAddNewEntryToRoutingTableWindowCommand { get; }
        public RouterDeviceViewModel(DeviceOnCanvas device, IWindowNavigator windowNavigator, IMediator mediator) : base(device) 
        {
            _windownavigator = windowNavigator;
            RouterDevice = device.LogicDevice as RouterDevice;
            RouterInterfaceViewModel = new ObservableCollection<RouterNetworkInterfaceViewModel>(
             RouterDevice.NetworkConfig.Interfaces.Select(i => new RouterNetworkInterfaceViewModel(i, mediator))
           );
            OpenAddNewEntryToRoutingTableWindowCommand = new RelayCommand(OpenAddNewEntryToRoutingTableWindow);
        }
        private void OpenAddNewEntryToRoutingTableWindow()
        {
            var vm = _windownavigator.ShowModalViewReturnViewModel<AddEntryToRoutingTableWindow, AddEntryToRoutingTableViewModel>();
        }
    }
}
