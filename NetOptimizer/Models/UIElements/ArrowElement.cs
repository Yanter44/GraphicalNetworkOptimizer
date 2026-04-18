using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NetOptimizer.Models.UIElements
{
    public class ArrowElement : UIElementBase
    {
        private Point _tip1;
        private Point _tip2;
        private Point _startPoint;
        private Point _endPoint;
        public Point Start { get => _startPoint; set { if (_startPoint != value) { _startPoint = value; UpdateArrowHead(); OnPropertyChanged(nameof(Start)); } }}
        public Point End { get => _endPoint; set { if (_endPoint != value) {_endPoint = value;  UpdateArrowHead(); OnPropertyChanged(nameof(End)); }}}
        public Point Tip1 { get => _tip1; set { _tip1 = value; OnPropertyChanged(nameof(Tip1)); } }
        public Point Tip2 { get => _tip2; set { _tip2 = value; OnPropertyChanged(nameof(Tip2)); } }

        public void UpdateArrowHead()
        {
            double angle = Math.Atan2(End.Y - Start.Y, End.X - Start.X);
            double length = 10;

            Tip1 = new Point(End.X - length * Math.Cos(angle - Math.PI / 6),
                             End.Y - length * Math.Sin(angle - Math.PI / 6));
            Tip2 = new Point(End.X - length * Math.Cos(angle + Math.PI / 6),
                             End.Y - length * Math.Sin(angle + Math.PI / 6));
        }
    }
}
