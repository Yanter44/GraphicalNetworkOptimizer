using NetOptimizer.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetOptimizer.Models.Dtos
{
    public class UIToolElementToAddDto : INotifyPropertyChanged
    {
        public string ToolName { get; set; }
        public UIToolElementType Type { get; set; }
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected));}}

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
