using NetOptimizer.Common;
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
        public ICommand CloseCommand { get; }
        public AddEntryToRoutingTableViewModel()
        {
            CloseCommand = new RelayCommand(CloseWindow);
        }
        private void CloseWindow() => RequestClose?.Invoke();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
