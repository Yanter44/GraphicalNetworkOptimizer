using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace NetOptimizer.Models.UIElements
{
    public class DeviceConnection : INotifyPropertyChanged
    {
        public int ConnectionIndex { get; set; }
        public ConnectionPoint StartPoint { get; set; } = new ConnectionPoint();
        public ConnectionPoint EndPoint { get; set; } = new ConnectionPoint();
        public double ArrowAngle { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public Port SourcePort { get; set; }
        public Port TargetPort { get; set; }

        private DeviceOnCanvas _source;
        public DeviceOnCanvas Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    if (_source != null) _source.PropertyChanged -= Device_PropertyChanged;
                    _source = value;
                    if (_source != null) _source.PropertyChanged += Device_PropertyChanged;
                    OnPropertyChanged(nameof(Source));
                    UpdateLine();
                }
            }
        }

        private DeviceOnCanvas _target;
        public DeviceOnCanvas Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    if (_target != null) _target.PropertyChanged -= Device_PropertyChanged;
                    _target = value;
                    if (_target != null) _target.PropertyChanged += Device_PropertyChanged;
                    OnPropertyChanged(nameof(Target));
                    UpdateLine();
                }
            }
        }

        public void Device_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeviceOnCanvas.X) || e.PropertyName == nameof(DeviceOnCanvas.Y))
            {
                UpdateLine();
            }
        }

        public void UpdateLine()
        {
            if (Source == null || Target == null) return;

            double ax = Source.X + 32;
            double ay = Source.Y + 32;

            double bx = Target.X + 32;
            double by = Target.Y + 32;

            double dx = bx - ax;
            double dy = by - ay;

            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len == 0) return;

            double nx = -dy / len;
            double ny = dx / len;

            double spacing = 7;
            double offset;

            if (ConnectionIndex < 3)
                offset = (ConnectionIndex - 0.2) * spacing;
            else
                offset = -(ConnectionIndex - 2) * spacing;

            double ox = nx * offset;
            double oy = ny * offset;

            double x1 = ax + ox;
            double y1 = ay + oy;

            double x2 = bx + ox;
            double y2 = by + oy;

            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;

            double tStart = 0.2;
            double tEnd = 0.8;

            StartPoint.Position = new Point(x1 + tStart * (x2 - x1), y1 + tStart * (y2 - y1));
            EndPoint.Position = new Point(x1 + tEnd * (x2 - x1),y1 + tEnd * (y2 - y1));
            ArrowAngle = Math.Atan2(Y2 - Y1, X2 - X1) * 180 / Math.PI;

            OnPropertyChanged(nameof(ArrowAngle));
            OnPropertyChanged(nameof(StartPoint));
            OnPropertyChanged(nameof(EndPoint));
            OnPropertyChanged(nameof(X1));
            OnPropertyChanged(nameof(Y1));
            OnPropertyChanged(nameof(X2));
            OnPropertyChanged(nameof(Y2));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
