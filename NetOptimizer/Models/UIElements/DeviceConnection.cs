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

            double dx = Target.X - Source.X;
            double dy = Target.Y - Source.Y;

            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len == 0) return;

            double nx = -dy / len;
            double ny = dx / len;

            double spacing = 7;
            double offset = 0;
            if (ConnectionIndex < 3)
            {
                offset = (ConnectionIndex - 0.2) * spacing;
            }
            else
            {

                offset = -(ConnectionIndex - 2) * spacing;
            }

            double ox = nx * offset;
            double oy = ny * offset;

            double cx1 = Source.X + 32 + ox;
            double cy1 = Source.Y + 32 + oy;

            double cx2 = Target.X + 32 + ox;
            double cy2 = Target.Y + 32 + oy;

            X1 = cx1;
            Y1 = cy1;
            X2 = cx2;
            Y2 = cy2;

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
