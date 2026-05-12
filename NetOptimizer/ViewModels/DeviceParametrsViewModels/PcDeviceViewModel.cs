using MediatR;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.PC;
using System.Collections.ObjectModel;

namespace NetOptimizer.ViewModels.DeviceParametrsViewModels
{
    public class PcDeviceViewModel : DeviceViewModelBase, IApplyChangesVm
    {
        private PcDevice PcDevice;
        public ObservableCollection<PcNetworkInterfaceViewModel> PcInterfaceViewModel { get; set; } = new();
        public ObservableCollection<ArpTableEntryViewModel> ArpTableViewModel { get; set; } = new();

        public PcDeviceViewModel(DeviceOnCanvas device, IMediator mediator) : base(device)
        {
            PcDevice = device.LogicDevice as PcDevice;

            PcInterfaceViewModel = new ObservableCollection<PcNetworkInterfaceViewModel>(
              PcDevice.NetworkConfig.Interfaces.Select(i => new PcNetworkInterfaceViewModel(i, mediator))
            );
            ArpTableViewModel = new ObservableCollection<ArpTableEntryViewModel>(
                        PcDevice.RuntimeState.ArpTable.Select(i => new ArpTableEntryViewModel
                        {
                          IpAddress = i.IpAddress,
                          MacAddress = i.MacAddress
                        })
            );

        }
        public void ApplyChanges()
        {
          
        }
    }
}
