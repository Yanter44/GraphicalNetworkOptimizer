using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace NetOptimizer.Models.UIElements
{
    public class ConnectionPoint : INotifyPropertyChanged
    {
        public ConnectionPoint(PointConnectionState initialState = PointConnectionState.Negotiating)
        {
            _state = initialState;
        }
        private Point _position;
        public Point Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged();
                }
            }
        }

        private PointConnectionState _state;
        public PointConnectionState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
