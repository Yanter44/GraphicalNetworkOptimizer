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
        public ObservableCollection<PcNetworkInterfaceViewModel> PcInterfaceViewModel { get; set; }
        public PcDeviceViewModel(DeviceOnCanvas device) : base(device)
        {
            PcDevice = device.LogicDevice as PcDevice;

            PcInterfaceViewModel = new ObservableCollection<PcNetworkInterfaceViewModel>(
              PcDevice.NetworkConfig.Interfaces.Select(i => new PcNetworkInterfaceViewModel(i))
            );
        }
        public void ApplyChanges()
        {
          
        }
    }
}
