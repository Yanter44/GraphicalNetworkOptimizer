using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.Switch;
using NetOptimizer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public class SwitchDeviceViewModel : DeviceViewModelBase, IApplyChangesVm
    {
        private readonly IWindowNavigator _windownavigator;
        public ICommand OpenAddVlanCommand { get; }
        private SwitchDevice SwitchDevice;
        public ObservableCollection<NetworkInterfaceViewModel> SwitchInterfaceViewModel { get; set; }
        public ObservableCollection<SelectableVlan> VlansViewModel { get; set; }
        public ObservableCollection<MacTableEntry> MacTableViewModel { get; set; } = new ObservableCollection<MacTableEntry>();

        public SwitchDeviceViewModel(DeviceOnCanvas device, IWindowNavigator windownavigator) : base(device)
        {
            _windownavigator = windownavigator;
            SwitchDevice = device.LogicDevice as SwitchDevice;
            MacTableViewModel.CollectionChanged += (s, e) =>
            {
                SwitchDevice.RuntimeState.MacTable = MacTableViewModel.ToList();
            };
            VlansViewModel = new ObservableCollection<SelectableVlan>(
                 SwitchDevice.NetworkConfig.Vlans.Select(v => new SelectableVlan
                 {
                  VlanId = v.Id,
                  VlanName = v.Name
                 })
            );
            foreach (var iface in SwitchDevice.NetworkConfig.Interfaces)
            {
                if (iface.SwitchPortMode == SwitchPortMode.Trunk &&
                    iface.AllowedVlans != null)
                {
                    foreach (var vlanVm in VlansViewModel)
                    {
                        if (iface.AllowedVlans.Contains(vlanVm.VlanId))
                        {
                            vlanVm.IsSelected = true;
                        }
                    }
                }
            }
            SwitchInterfaceViewModel = new ObservableCollection<NetworkInterfaceViewModel>(
                SwitchDevice.NetworkConfig.Interfaces.Select(i => new NetworkInterfaceViewModel(i))
            );
            OpenAddVlanCommand = new RelayCommand(OpenAddVlan);
        }
        public void ApplyChanges()
        {
            var logicdevice = Device.LogicDevice as SwitchDevice;
            logicdevice.NetworkConfig.Vlans.Clear();

            foreach (var vlan in VlansViewModel)
            {
                logicdevice.NetworkConfig.Vlans.Add(new Vlan
                {
                    Id = vlan.VlanId,
                    Name = vlan.VlanName
                });
            }

            for (int i = 0; i < SwitchInterfaceViewModel.Count; i++)
            {
                var vm = SwitchInterfaceViewModel[i];
                var model = logicdevice.NetworkConfig.Interfaces[i];

                model.Name = vm.Name;
                model.IsEnabled = vm.IsEnabled;
                model.SwitchPortMode = vm.SwitchPortMode;

                if (vm.SwitchPortMode == SwitchPortMode.Access)
                {
                    model.AccessVlan = vm.AccessVlan;
                    model.AllowedVlans = null;
                }
                else if (vm.SwitchPortMode == SwitchPortMode.Trunk)
                {
                    model.AccessVlan = null;

                    model.AllowedVlans = VlansViewModel
                        .Where(v => v.IsSelected)
                        .Select(v => v.VlanId)
                        .ToList();
                }
            }
        }
        private void OpenAddVlan()
        {
            var vm = _windownavigator.ShowModalViewReturnViewModel<CreateVlanOnDeviceWindow, CreateVlanOnDeviceViewModel>();
            vm.VlanConfirmed += (VlanId, VlanName) =>
            {
                VlansViewModel.Add(new SelectableVlan() { VlanId = int.Parse(VlanId), VlanName = VlanName });
                vm.CloseCommand.Execute(null);
            };        
        }
    }
}
