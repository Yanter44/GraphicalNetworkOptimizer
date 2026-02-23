using NetOptimizer.Common;
using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace NetOptimizer.ViewModels
{
    public class ErrorWindowViewModel : INotifyPropertyChanged, IModalWindow
    {
        private string _message;
        public string Message { get { return _message; } set { if (value != null) { _message = value; OnPropertyChanged(); } } }

        public ICommand CloseCommand { get; }
        public ErrorWindowViewModel()
        {
            CloseCommand = new RelayCommand(obj => { if (obj is Window window) { window.Close(); } });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
