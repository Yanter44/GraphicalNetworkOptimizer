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

        public string FromDeviceId { get; set; }
        public string ToDeviceId { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

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

        public DeviceOnCanvas FromDevice { get; set; }
        public DeviceOnCanvas ToDevice { get; set; }

        private void UpdatePosition()
        {
            if (FromDevice == null || ToDevice == null)
                return;

            X = FromDevice.X + (ToDevice.X - FromDevice.X) * Progress;
            Y = FromDevice.Y + (ToDevice.Y - FromDevice.Y) * Progress;

            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
