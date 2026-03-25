using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.ViewModels.MainWindoww;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace NetOptimizer.Behaviors
{
    public class CanvasInteractionBehavior : Behavior<Canvas>
    {
        private Point _selectionStart;
        private Point _lastMouse;
        private bool _isPanning;
        public Polyline _connectionLine;
        private DeviceOnCanvas _sourceDevice;
        private Port _sourcePort;
        public bool IsTryingToConnect => _connectionLine != null;
        public event Action<DeviceOnCanvas, DeviceOnCanvas> ConnectionCreated;
        protected override void OnAttached()
        {
            AssociatedObject.MouseWheel += OnWheel;
            AssociatedObject.MouseDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseUp += OnMouseUp;
        }
        private CanvasViewModel VM => AssociatedObject.DataContext as CanvasViewModel;
        public void StartConnectionLine(Point start, Polyline line, DeviceOnCanvas sourceDevice, Port sourcePort)
        {
            _connectionLine = line;
            _connectionLine.Points.Clear();
            _connectionLine.Points.Add(start);
            _connectionLine.Points.Add(start);
            _connectionLine.Visibility = Visibility.Visible;

            _sourceDevice = sourceDevice;
            _sourcePort = sourcePort;
        }

        public void FinishConnection(DeviceOnCanvas targetDevice, Port targetPort)
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window?.DataContext is MainWindowViewModel vm && _sourceDevice != null && _sourcePort != null)
            {
                if (vm.TryConnectPorts(_sourceDevice, _sourcePort, targetDevice, targetPort, _connectionLine?.Points))
                {
                    StopConnectionLine();
                    ConnectionCreated?.Invoke(_sourceDevice, targetDevice);
                }
            }
        }
        private void OnWheel(object sendesr, MouseWheelEventArgs e)
        {
            if (VM == null) return;

            double delta = e.Delta > 0 ? 1 : -1;
            VM.Zoom(delta);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (VM == null) return;

            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is DeviceOnCanvas)
                return;

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isPanning = true;
                _lastMouse = e.GetPosition((UIElement)AssociatedObject.Parent);
                AssociatedObject.CaptureMouse();
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_connectionLine != null)
                {
                    Point pos = e.GetPosition(AssociatedObject);
                    _connectionLine.Points[_connectionLine.Points.Count - 1] = pos;
                    _connectionLine.Points.Add(pos);
                }
                else
                {
                    _selectionStart = e.GetPosition(AssociatedObject);
                    VM.StartSelection(_selectionStart);
                }   
            }
            if(e.RightButton == MouseButtonState.Pressed)
            {
                if(_connectionLine != null)
                {
                    StopConnectionLine();
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (VM == null) return;
            if (_connectionLine != null)
            {
                Point pos = e.GetPosition(AssociatedObject);

                if (_connectionLine.Points.Count > 0)
                {
                    _connectionLine.Points[_connectionLine.Points.Count - 1] = pos;
                }
            }
            if (_isPanning)
            {
                Point current = e.GetPosition((UIElement)AssociatedObject.Parent);
                Vector delta = current - _lastMouse;
                VM.Pan(delta);
                _lastMouse = current;
            }

            if (VM.IsSelecting)
            {
                Point current = e.GetPosition(AssociatedObject);
                VM.UpdateSelection(_selectionStart, current);
            }
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;

            if (VM.IsSelecting)
                VM.StopSelection();

            AssociatedObject.ReleaseMouseCapture();
        }
        public void StopConnectionLine()
        {
            if (_connectionLine == null) return;

            _connectionLine.Points.Clear();
            _connectionLine.Visibility = Visibility.Collapsed;
            _connectionLine = null;
        }
    }
}
