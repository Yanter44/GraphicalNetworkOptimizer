using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace NetOptimizer.Models
{
    public class DeviceConnection : INotifyPropertyChanged
    {
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
                    UpdatePoints();
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
                    UpdatePoints();
                }
            }
        }

        public Port SourcePort { get; set; }
        public Port TargetPort { get; set; }

        private PointCollection _points = new PointCollection();
        public PointCollection Points
        { get => _points;
          private set { _points = value; OnPropertyChanged(); } }

        public DeviceConnection()
        {
            _points = new PointCollection();
        }

        public void Device_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeviceOnCanvas.X) || e.PropertyName == nameof(DeviceOnCanvas.Y))
            {
                UpdateEdgePoints(sender as DeviceOnCanvas);
            }
        }

        private void UpdateEdgePoints(DeviceOnCanvas movedDevice)
        {
            if (_points.Count < 2) return;
            var newPoints = new PointCollection(_points);

            if (movedDevice == Source)
                newPoints[0] = new Point(Source.X + 32, Source.Y + 32);
            else if (movedDevice == Target)
                newPoints[newPoints.Count - 1] = new Point(Target.X + 32, Target.Y + 32);

            Points = newPoints; 
        }

        public void UpdatePoints()
        {
            if (Source == null || Target == null) return;

            var newPoints = new PointCollection(_points.Count >= 2 ? _points : new[] {
                    new Point(Source.X + 32, Source.Y + 32),
                    new Point(Target.X + 32, Target.Y + 32)
             });

            newPoints[0] = new Point(Source.X + 32, Source.Y + 32);
            newPoints[newPoints.Count - 1] = new Point(Target.X + 32, Target.Y + 32);

            Points = newPoints;
        }
        public void AddIntermediatePoint(Point p)
        {
            if (_points.Count > 1)
                _points.Insert(_points.Count - 1, p);
            else
                _points.Add(p);

            OnPropertyChanged(nameof(Points));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
