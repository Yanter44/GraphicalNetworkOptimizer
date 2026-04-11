using NetOptimizer.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetOptimizer.Models.UIElements
{
    public abstract class UIElementBase : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        public double X { get => _x; set { if (_x != value) { _x = value; OnPropertyChanged(); } } }
        public double Y { get => _y; set { if (_y != value) { _y = value; OnPropertyChanged(); } } }

        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }
        public UIToolElementType Type { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
