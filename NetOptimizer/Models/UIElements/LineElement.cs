using System.ComponentModel;
using System.Windows;

namespace NetOptimizer.Models.UIElements
{
    public class LineElement : UIElementBase
    {
        private Point _startPoint;
        private Point _endPoint;
        public Point Start
        {
            get => _startPoint;
            set
            {
                if (_startPoint != value)
                {
                    _startPoint = value;
                    OnPropertyChanged(nameof(Start));
                }
            }
        }
        public Point End
        {
            get => _endPoint;
            set
            {
                if (_endPoint != value)
                {
                    _endPoint = value;
                    OnPropertyChanged(nameof(End));
                }
            }
        }
    }
}
