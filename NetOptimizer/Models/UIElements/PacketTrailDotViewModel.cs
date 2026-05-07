using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.UIElements
{
    public class PacketTrailDotViewModel : INotifyPropertyChanged
    {
        public string PacketId { get; set; }
        private double _x;
        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
