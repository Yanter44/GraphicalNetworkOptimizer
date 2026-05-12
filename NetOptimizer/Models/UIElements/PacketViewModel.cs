using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.UIElements
{
    public class PacketViewModel : INotifyPropertyChanged
    {
        public string PacketId { get; set; }
        public PacketType PacketType { get; set; }

        public DeviceConnection Connection { get; set; }
        public bool Reversed { get; set; }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
                UpdatePosition();
            }
        }

        public double X { get; set; }
        public double Y { get; set; }

        private void UpdatePosition()
        {
            if (Connection == null)
                return;

            var x1 = Reversed ? Connection.X2 : Connection.X1;
            var y1 = Reversed ? Connection.Y2 : Connection.Y1;

            var x2 = Reversed ? Connection.X1 : Connection.X2;
            var y2 = Reversed ? Connection.Y1 : Connection.Y2;

            X = x1 + (x2 - x1) * Progress;
            Y = y1 + (y2 - y1) * Progress;

            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
