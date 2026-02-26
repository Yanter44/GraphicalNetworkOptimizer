using NetOptimizer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NetOptimizer.Views.MainWindow
{
    public partial class MainWindow
    {
        private Point _selectionStartPoint;
        private Point _lastMousePosition;

        private bool _isPanning = false;
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomSpeed = 0.1;
            double zoomChange = e.Delta > 0 ? zoomSpeed : -zoomSpeed;
            double newScale = CanvasScale.ScaleX + zoomChange;

            if (newScale >= 0.3 && newScale <= 3.0)
            {
                CanvasScale.ScaleX = newScale;
                CanvasScale.ScaleY = newScale;
            }
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Right)
            {
                _isPanning = true;
                _lastMousePosition = e.GetPosition(this);
                MainCanvas.CaptureMouse();
                return;
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_connectionLine != null)
                {

                    MainCanvas.Children.Remove(_connectionLine);
                    _connectionLine = null;
                    _sourceDeviceConnection = null;

                    MainCanvas.ReleaseMouseCapture();
                    return;
                }
                _selectionStartPoint = e.GetPosition(MainCanvas);
                var vm = (MainWindowViewModel)this.DataContext;

                if (e.Source == MainCanvas)
                {
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        foreach (var device in vm.DevicesOnCanvas)
                        {
                            device.IsSelected = false;

                            var visual = MainCanvas.Children.OfType<FrameworkElement>()
                                         .FirstOrDefault(x => x.Tag == device);

                            if (visual is Panel panel) panel.Background = Brushes.Transparent;
                            if (visual is Border border) border.Background = Brushes.Transparent;
                        }
                    }
                }
                Canvas.SetLeft(SelectionBox, _selectionStartPoint.X);
                Canvas.SetTop(SelectionBox, _selectionStartPoint.Y);
                SelectionBox.Width = 0;
                SelectionBox.Height = 0;
                SelectionBox.Visibility = Visibility.Visible;

                MainCanvas.CaptureMouse();
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_connectionLine != null)
            {
                Point mousePos = e.GetPosition(MainCanvas);
                _connectionLine.X2 = mousePos.X - 10;
                _connectionLine.Y2 = mousePos.Y;
            }
            else if (_isPanning)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - _lastMousePosition;
                CanvasTranslate.X += delta.X;
                CanvasTranslate.Y += delta.Y;
                _lastMousePosition = currentPosition;
            }
            else if (SelectionBox.Visibility == Visibility.Visible)
            {
                Point currentPos = e.GetPosition(MainCanvas);
                double x = Math.Min(_selectionStartPoint.X, currentPos.X);
                double y = Math.Min(_selectionStartPoint.Y, currentPos.Y);
                double width = Math.Abs(currentPos.X - _selectionStartPoint.X);
                double height = Math.Abs(currentPos.Y - _selectionStartPoint.Y);

                Canvas.SetLeft(SelectionBox, x);
                Canvas.SetTop(SelectionBox, y);
                SelectionBox.Width = width;
                SelectionBox.Height = height;

                Rect selectionRect = new Rect(x, y, width, height);
                var vm = (MainWindowViewModel)this.DataContext;

                foreach (var device in vm.DevicesOnCanvas)
                {
                    Rect deviceRect = new Rect(device.X, device.Y, 64, 64);
                    bool isIntersecting = selectionRect.IntersectsWith(deviceRect);
                    var visualElement = MainCanvas.Children.OfType<StackPanel>()
                        .FirstOrDefault(b => b.Tag == device);

                    if (visualElement != null)
                    {
                        if (isIntersecting)
                        {
                            visualElement.Background = new SolidColorBrush(Color.FromArgb(50, 0, 120, 215));
                            device.IsSelected = true;
                        }
                        else
                        {
                            visualElement.Background = Brushes.Transparent;
                            device.IsSelected = false;
                        }
                    }
                }
            }
        }
        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;

            if (SelectionBox.Visibility == Visibility.Visible)
            {
                SelectionBox.Visibility = Visibility.Collapsed;
            }

            MainCanvas.ReleaseMouseCapture();
        }
    }
}
