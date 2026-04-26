using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww
{
    public class CreateVlanOnDeviceViewModel : INotifyPropertyChanged
    {
        private string _vlanId;
        private string _vlanName;
        public string VlanId { get => _vlanId; set { _vlanId = value; OnPropertyChanged(); } }
        public string VlanName { get => _vlanName; set { _vlanName = value; OnPropertyChanged(); } }
        public Device EditableDevice { get; set; }

        public ICommand CloseCommand { get; }
        public ICommand ConfirmAndAddVlanCommand { get; }

        public event Action RequestClose;

        public CreateVlanOnDeviceViewModel(Device device)
        {
            EditableDevice = device;
            CloseCommand = new RelayCommand(CloseWindow);
            ConfirmAndAddVlanCommand = new RelayCommand(obj => ConfirmAndAddVlan());
        }
        private void ConfirmAndAddVlan()
        {
            if (string.IsNullOrWhiteSpace(VlanId) || !int.TryParse(VlanId, out int id))
                return;

            IList<Vlan> vlans = null;

            if (EditableDevice is SwitchDevice sw)
            {
                vlans = sw.NetworkConfig.Vlans;
            }
            else if (EditableDevice is RouterDevice router)
            {
                // vlans = router.NetworkConfig.Vlans;
            }

            if (vlans == null) return;
            var existingVlan = vlans.FirstOrDefault(v => v.Id == id);

            if (existingVlan != null)
            {
                if (existingVlan.Name != VlanName)
                {
                    existingVlan.Name = VlanName;
                }
            }
            else
            {
                vlans.Add(new Vlan { Id = id, Name = VlanName });
            }
            CloseWindow();
        }

        private void CloseWindow() => RequestClose?.Invoke();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
