using MediatR;
using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.AddRouteEntryToRoutingTableWindoww;
using NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww;
using NetOptimizer.ViewModels.DeleteEntryInRoutingTableWindoww;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.PC;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.Router;
using NetOptimizer.Views;
using NetOptimizer.Views.AddNewEntryToRoutingTableWindow;
using NetOptimizer.Views.DeleteEntryInRoutingTableWindow;
using NetOptimizer.Views.DopViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public class RouterDeviceViewModel : DeviceViewModelBase, IApplyChangesVm
    {
        private readonly IWindowNavigator _windownavigator;
        private RouterDevice RouterDevice;
        public ObservableCollection<RouterNetworkInterfaceViewModel> RouterInterfaceViewModel { get; set; } = new();
        public ObservableCollection<ArpTableEntryViewModel> ArpTableViewModel { get; set; } = new();
        public ObservableCollection<StaticRoutingNetworkTableEntryViewModel> StaticRoutingNetworkTableViewModel { get; set; } = new();
        public ObservableCollection<AutomaticRoutingNetworkTableViewModel> AutomaticRoutingNetworkTableViewModels { get; set; } = new();
        public ICommand OpenAddNewEntryToRoutingTableWindowCommand { get; }
        public ICommand OpenDeleteEntryInRoutingTableWindowCommand { get; }
        public RouterDeviceViewModel(DeviceOnCanvas device, IWindowNavigator windowNavigator, IMediator mediator) : base(device) 
        {
            _windownavigator = windowNavigator;
            RouterDevice = device.LogicDevice as RouterDevice;
            RouterInterfaceViewModel = new ObservableCollection<RouterNetworkInterfaceViewModel>(
                RouterDevice.NetworkConfig.Interfaces.Select(i => new RouterNetworkInterfaceViewModel(i, mediator))
            );
            ArpTableViewModel = new ObservableCollection<ArpTableEntryViewModel>(
                RouterDevice.RuntimeState.ArpTable.Select(i => new ArpTableEntryViewModel() 
                { 
                    IpAddress = i.IpAddress,
                    MacAddress = i.MacAddress,
                })
            );
            StaticRoutingNetworkTableViewModel = new ObservableCollection<StaticRoutingNetworkTableEntryViewModel>(
                RouterDevice.RuntimeState.StaticRoutingTable.Select(i =>
                {
                    var iface = RouterDevice.NetworkConfig.Interfaces.FirstOrDefault(x => x.Id == i.InterfaceId);
                    return new StaticRoutingNetworkTableEntryViewModel
                    {
                        DestinationNetwork = i.DestinationNetwork,
                        SubnetMask = i.SubnetMask,
                        NextHopIp = i.NextHopIp,
                        InterfaceName = iface?.Name ?? "Unknown"
                    };
                })
            );

            AutomaticRoutingNetworkTableViewModels = new ObservableCollection<AutomaticRoutingNetworkTableViewModel>(
                RouterDevice.RuntimeState.AutomaticRoutingTable.Select(i =>
                {
                    var iface = RouterDevice.NetworkConfig.Interfaces.FirstOrDefault(x => x.Id == i.InterfaceId);
                    return new AutomaticRoutingNetworkTableViewModel
                    {
                        Network = i.Network,
                        SubnetMask = i.SubnetMask,
                        InterfaceName = iface?.Name ?? "Unknow"
                    };
                })
            );
            OpenAddNewEntryToRoutingTableWindowCommand = new RelayCommand(OpenAddNewEntryToRoutingTableWindow);
            OpenDeleteEntryInRoutingTableWindowCommand = new RelayCommand(OpenDeleteEntryInRoutingTableWindow);
        }
        private void OpenAddNewEntryToRoutingTableWindow()
        {
            var vm = _windownavigator
                     .ShowModalViewReturnViewModel<AddEntryToRoutingTableWindow, AddEntryToRoutingTableViewModel>();

            vm.RouteConfirmed += (routeDto) =>
            {
                var entry = new StaticRouteEntry
                {
                    DestinationNetwork = routeDto.Network,
                    SubnetMask = routeDto.SubnetMask,
                    NextHopIp = routeDto.NextHopIp
                };
                var result = RouterDevice.TryAddStaticRoute(entry);
                if (!result.Success)
                {
                    _windownavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>(result.Error);
                    return;
                }

                StaticRoutingNetworkTableViewModel.Add(new StaticRoutingNetworkTableEntryViewModel
                {
                    DestinationNetwork = entry.DestinationNetwork,
                    SubnetMask = entry.SubnetMask,
                    NextHopIp = entry.NextHopIp,
                    InterfaceName = result.InterfaceName
                });
                vm.CloseCommand.Execute(null);
            };
        }
        private void OpenDeleteEntryInRoutingTableWindow()
        {
            DeleteEntryInRoutingTableViewModel? vm = null;

            _windownavigator.ShowModalView<DeleteEntryInRoutingTableWindow,DeleteEntryInRoutingTableViewModel>(x =>
            {
                vm = x;
                x.Routes = StaticRoutingNetworkTableViewModel;
                x.RouteDeleted += OnRouteDeleted;
            });

            if (vm != null)
            {
                vm.RouteDeleted -= OnRouteDeleted;
            }
        }

        private void OnRouteDeleted(StaticRoutingNetworkTableEntryViewModel route)
        {
            StaticRoutingNetworkTableViewModel.Remove(route);
        }

        public void ApplyChanges()
        {
            var logicdevice = Device.LogicDevice as RouterDevice;

            var toRemove = logicdevice.RuntimeState.StaticRoutingTable.Where(r => !StaticRoutingNetworkTableViewModel.Any(vm =>
                    vm.DestinationNetwork == r.DestinationNetwork &&
                    vm.SubnetMask == r.SubnetMask &&
                    vm.NextHopIp == r.NextHopIp))
                .ToList();

            if (toRemove.Any())
            {
                foreach (var route in toRemove)
                {
                    logicdevice.RuntimeState.StaticRoutingTable.Remove(route);
                }

            }
            foreach (var routingentry in StaticRoutingNetworkTableViewModel)
            {
                bool exists = logicdevice.RuntimeState.StaticRoutingTable.Any(r =>
                    r.DestinationNetwork == routingentry.DestinationNetwork &&
                    r.SubnetMask == routingentry.SubnetMask &&
                    r.NextHopIp == routingentry.NextHopIp);

                if (!exists)
                {
                    logicdevice.RuntimeState.StaticRoutingTable.Add(new StaticRouteEntry
                    {
                        DestinationNetwork = routingentry.DestinationNetwork,
                        SubnetMask = routingentry.SubnetMask,
                        NextHopIp = routingentry.NextHopIp
                    });
                }
            }
            logicdevice.RebuildRoutingTable();
        }
    }
}
