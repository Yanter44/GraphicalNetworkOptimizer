using NetOptimizer.Common;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.AddRouteEntryToRoutingTableWindoww
{
    public class AddEntryToRoutingTableViewModel : INotifyPropertyChanged
    {
        public event Action RequestClose;
        public event Action<RouteDto> RouteConfirmed;
        public ICommand CloseCommand { get; }
        public ICommand ConfirmAndAddRouteEntryCommand { get; }

        private string _network;
        public string Network { get => _network; set { _network = value; OnPropertyChanged(); } }

        private string _subnetMask;
        public string SubnetMask { get => _subnetMask; set { _subnetMask = value; OnPropertyChanged(); } }

        private string _nextHopIp;
        public string NextHopIp { get => _nextHopIp; set { _nextHopIp = value; OnPropertyChanged(); } }
        
        public AddEntryToRoutingTableViewModel()
        {
            CloseCommand = new RelayCommand(CloseWindow);
            ConfirmAndAddRouteEntryCommand = new RelayCommand(ConfirmAndAddRouteEntry);
        }
        private void ConfirmAndAddRouteEntry()
        {
            RouteConfirmed.Invoke(new RouteDto()
            {
                Network = Network,
                NextHopIp = NextHopIp,
                SubnetMask = SubnetMask,
            });
        }
        private void CloseWindow() => RequestClose?.Invoke();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
