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
        public ICommand CloseCommand { get; }
        public ICommand ConfirmAndAddVlanCommand { get; }
        public event Action<string, string> VlanConfirmed;
        public event Action RequestClose;

        public CreateVlanOnDeviceViewModel()
        {
            CloseCommand = new RelayCommand(CloseWindow);
            ConfirmAndAddVlanCommand = new RelayCommand(obj => ConfirmAndAddVlan());
        }
        private void ConfirmAndAddVlan()
        {
            VlanConfirmed?.Invoke(VlanId, VlanName);
        }

        private void CloseWindow() => RequestClose?.Invoke();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
