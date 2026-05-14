using NetOptimizer.Common;
using NetOptimizer.ViewModels.DeviceParametrsViewModels.Router;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.DeleteEntryInRoutingTableWindoww
{
    public class DeleteEntryInRoutingTableViewModel : INotifyPropertyChanged
    {
        public event Action RequestClose;
        public event Action<StaticRoutingNetworkTableEntryViewModel> RouteDeleted;
        private StaticRoutingNetworkTableEntryViewModel _selectedRoute;
        public ObservableCollection<StaticRoutingNetworkTableEntryViewModel> Routes { get; set;}
        public StaticRoutingNetworkTableEntryViewModel SelectedRoute { get => _selectedRoute; set { _selectedRoute = value;  OnPropertyChanged();}}
        public ICommand ConfirmCommand { get; }
        public ICommand CloseCommand { get; }
        public DeleteEntryInRoutingTableViewModel()
        {
            ConfirmCommand = new RelayCommand(DeleteRoute);
            CloseCommand = new RelayCommand(CloseWindow);
        }
        private void DeleteRoute()
        {
            if (SelectedRoute == null)
                return;

            RouteDeleted?.Invoke(SelectedRoute);
            CloseWindow();
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
