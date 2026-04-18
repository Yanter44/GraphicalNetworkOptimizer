using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace NetOptimizer.Models.UIElements
{
    public class TempLine : INotifyPropertyChanged
    {
        private Point _start;
        private Point _end;
        public Point Start { get => _start; set { _start = value; OnPropertyChanged(); } }
        public Point End { get => _end; set { _end = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
