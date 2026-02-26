using NetOptimizer.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetOptimizer.Models.Dtos
{
    public class AvailableTypesOfObjectForEditorDto : INotifyPropertyChanged
    {
        public string TypeOfObject { get; set; }
        public PlacementType Type { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
