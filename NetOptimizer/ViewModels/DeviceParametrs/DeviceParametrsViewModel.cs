using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.CreateDeviceWindoww;
using NetOptimizer.ViewModels.CreateVlanOnDeviceWindoww;
using NetOptimizer.ViewModels.DeviceParametrsViewModels;
using NetOptimizer.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace NetOptimizer.ViewModels.DeviceParametrs
{
    public class DeviceParametrsViewModel : INotifyPropertyChanged
    {
        public event Action RequestClose;
        public DeviceViewModelBase DeviceVm { get;  }

        public ICommand CloseCommand { get; }
        private readonly IWindowNavigator _windowNavigator;
        public DeviceParametrsViewModel(DeviceViewModelFactory factory,
                                       DeviceOnCanvas device, IWindowNavigator windowNavigator)
        {
            _windowNavigator = windowNavigator;
            DeviceVm = factory.Create(device);
            CloseCommand = new RelayCommand(w =>
            {
                RequestClose.Invoke();
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
