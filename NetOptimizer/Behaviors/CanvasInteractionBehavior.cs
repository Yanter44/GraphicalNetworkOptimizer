using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.ViewModels.MainWindoww;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace NetOptimizer.Behaviors
{
    public class CanvasInteractionBehavior : Behavior<Canvas>
    {
        private Point _selectionStart;
        private Point _lastMouse;

        private Point _drawStart;
        private UIElement _drawElement;

        private bool _isPanning;
        public  Line _connectionLine;
        private DeviceOnCanvas _sourceDevice;
        private Port _sourcePort;
        public bool IsTryingToConnect => _connectionLine != null;
        public event Action<DeviceOnCanvas, DeviceOnCanvas> ConnectionCreated;
        private CanvasViewModel VM => AssociatedObject.DataContext as CanvasViewModel;
        protected override void OnAttached()
        {
            AssociatedObject.MouseWheel += OnWheel;
            AssociatedObject.MouseDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseUp += OnMouseUp;
            AssociatedObject.KeyDown += OnKeyDown;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                VM?.DeleteSelected();
            }
        }
        public void StartConnectionLine(Point start, Line line, DeviceOnCanvas sourceDevice, Port sourcePort)
        {
            _connectionLine = line;

            _connectionLine.X1 = start.X;
            _connectionLine.Y1 = start.Y;
            _connectionLine.X2 = start.X;
            _connectionLine.Y2 = start.Y;
            _connectionLine.IsHitTestVisible = false;
            _connectionLine.Visibility = Visibility.Visible;

            _sourceDevice = sourceDevice;
            _sourcePort = sourcePort;
        }

        public void FinishConnection(DeviceOnCanvas targetDevice, Port targetPort)
        {
            var window = Window.GetWindow(AssociatedObject);

            if (window?.DataContext is MainWindowViewModel vm &&
                _sourceDevice != null &&
                _sourcePort != null)
            {
                if (VM.TryConnectPorts(_sourceDevice, _sourcePort, targetDevice, targetPort))
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
            AssociatedObject.Focus();
            if (VM == null) return;

            bool isDeviceClick = e.OriginalSource is FrameworkElement fe && fe.DataContext is DeviceOnCanvas;
            if (isDeviceClick)
                return;

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isPanning = true;
                _lastMouse = e.GetPosition((UIElement)AssociatedObject.Parent);
                AssociatedObject.CaptureMouse();
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (VM.CurrentTool != null && VM.CurrentTool.Type != UIToolElementType.Cursor)
                {
                    StartDrawing(e.GetPosition(AssociatedObject));
                    return;
                }
                if (VM.IsSelecting)
                {
                    foreach (var d in VM.Devices)
                        d.IsSelected = false;

                    foreach (var uiObject in VM.UIObjects)
                        uiObject.IsSelected = false;

                    VM.StopSelection();
                
                }
                _selectionStart = e.GetPosition(AssociatedObject);
                VM.StartSelection(_selectionStart);
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (_connectionLine != null)
                {
                    StopConnectionLine();
                }
            }
        }
        private void StartDrawing(Point start)
        {
            _drawStart = start;
            VM.IsDrawing = true;
            if (VM.CurrentTool == null) return;

            switch (VM.CurrentTool.Type)
            {
                case UIToolElementType.Label:
                    _drawElement = new TextBox
                    {
                        Foreground = Brushes.Black,
                        Width = 150,    
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                    };
                    break;
                case UIToolElementType.Square:
                    _drawElement = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    break;

                case UIToolElementType.Elipse:
                    _drawElement = new Ellipse
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    break;
                case UIToolElementType.Сurve:
                    var polyline = new Polyline
                    {
                        Points = new PointCollection(),
                        Stroke = Brushes.Black,       
                        StrokeThickness = 2          
                    };
                    _drawElement = polyline;
                    break;

                case UIToolElementType.Line:
                case UIToolElementType.Arrow:
                    _drawElement = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = start.X,
                        Y2 = start.Y
                    };
                    break;

                default:
                    return;
            }

            if (!(_drawElement is Line))
            {
                Canvas.SetLeft(_drawElement, start.X);
                Canvas.SetTop(_drawElement, start.Y);
            }
            AssociatedObject.Children.Add(_drawElement);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (VM == null) return;
            if (VM.IsDrawing)
            {
                Point current = e.GetPosition(AssociatedObject);

                double x = Math.Min(_drawStart.X, current.X);
                double y = Math.Min(_drawStart.Y, current.Y);

                double w = Math.Abs(current.X - _drawStart.X);
                double h = Math.Abs(current.Y - _drawStart.Y);

                switch (VM.CurrentTool.Type)
                {
                    case UIToolElementType.Square:
                        {
                            double size = Math.Max(w, h);

                            Canvas.SetLeft(_drawElement, x);
                            Canvas.SetTop(_drawElement, y);

                            if (_drawElement is Rectangle rect)
                            {
                                rect.Width = size;
                                rect.Height = size;
                            }
                            break;
                        }
                    case UIToolElementType.Elipse:
                        {
                            double size = Math.Max(w, h);

                            Canvas.SetLeft(_drawElement, x);
                            Canvas.SetTop(_drawElement, y);

                            if (_drawElement is Ellipse ellipse)
                            {
                                ellipse.Width = size;
                                ellipse.Height = size;
                            }
                            break;
                        }
                    case UIToolElementType.Сurve:
                        if (_drawElement is Polyline poly)
                        {
                            poly.Points.Add(new Point(current.X - Canvas.GetLeft(_drawElement),
                                                      current.Y - Canvas.GetTop(_drawElement)));
                        }
                        break;
                    case UIToolElementType.Line:
                    case UIToolElementType.Arrow:
                        {
                            if (_drawElement is Line line)
                            {
                                line.X2 = current.X;
                                line.Y2 = current.Y;
                            }
                            break;
                        }
                }
            }
            if (_connectionLine != null)
            {
                Point pos = e.GetPosition(AssociatedObject);
                _connectionLine.X2 = pos.X;
                _connectionLine.Y2 = pos.Y;
            }
            if (_isPanning)
            {
                Point current = e.GetPosition((UIElement)AssociatedObject.Parent);
                Vector delta = current - _lastMouse;
                VM.Pan(delta);
                _lastMouse = current;
            }

            if (VM.IsDrawingSelection)
            {
                Point current = e.GetPosition(AssociatedObject);
                VM.UpdateSelection(_selectionStart, current);
            }
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            if (VM.IsDrawing)
            {
                if (VM.CurrentTool.Type == UIToolElementType.Arrow)
                {
                    var arrow = _drawElement as Line;
                    AddArrowTip(arrow);
                }
                UIElementBase creatableObject = null;
                switch (_drawElement)
                {
                    case Rectangle rect:
                        creatableObject = new RectangleElement
                        {
                            Type = UIToolElementType.Square,
                            X = Canvas.GetLeft(rect),
                            Y = Canvas.GetTop(rect),
                            Width = rect.Width,
                            Height = rect.Height
                        };
                        break;

                    case Ellipse ellipse:
                        creatableObject = new EllipseElement
                        {
                            Type = UIToolElementType.Elipse,
                            X = Canvas.GetLeft(ellipse),
                            Y = Canvas.GetTop(ellipse),
                            Width = ellipse.Width,
                            Height = ellipse.Height
                        };
                        break;
                    case Line line:
                        creatableObject = new LineElement
                        {
                            Type = UIToolElementType.Line,
                            X = Canvas.GetLeft(line),
                            Y = Canvas.GetTop(line),
                            Start = new Point(line.X1, line.Y1),
                            End = new Point(line.X2, line.Y2)
                        };
                        break;
                    case Polyline poly:
                        creatableObject = new CurveElement
                        {
                            Type = UIToolElementType.Сurve,
                            X = Canvas.GetLeft(poly),
                            Y = Canvas.GetTop(poly),
                            Points = new PointCollection(poly.Points)
                        };
                        break;

                    case TextBox tb:
                        tb.Focus();
                        bool isFinalized = false;
                        tb.LostFocus += (s, args) =>
                        {
                            if (isFinalized) return;
                            isFinalized = true;
                            FinalizeTextEntry(tb);
                        };
                        tb.KeyDown += (s, args) =>
                        {
                            if (args.Key == Key.Enter)
                            {
                                if (isFinalized) return;
                                isFinalized = true;

                                FinalizeTextEntry(tb);
                            }
                        };

                        _drawElement = null;
                        VM.IsDrawing = false;
                        return;
                }     
                if (creatableObject != null)
                {
                    VM.UIObjects.Add(creatableObject);
                }
                if (_drawElement != null)
                {
                    AssociatedObject.Children.Remove(_drawElement);
                    _drawElement = null;
                }
                VM.IsDrawing = false;
            }
            if (VM.IsDrawingSelection)
            {
                VM.StopDrawSelection();
                if (!VM.Devices.Any(x => x.IsSelected) && !VM.UIObjects.Any(x => x.IsSelected))
                {
                    VM.StopSelection();
                }
            }
            AssociatedObject.ReleaseMouseCapture();
        }
        private void FinalizeTextEntry(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                AssociatedObject.Children.Remove(tb);
                return;
            }
            var label = new LabelElement
            {
                Type = UIToolElementType.Label,
                X = Canvas.GetLeft(tb),
                Y = Canvas.GetTop(tb),
                Width = tb.ActualWidth > 0 ? tb.ActualWidth : 100,
                Text = tb.Text
            };

            VM.UIObjects.Add(label);
            AssociatedObject.Children.Remove(tb);
        }
        public void StopConnectionLine()
        {
            if (_connectionLine == null) return;
            var canvas = AssociatedObject;
            canvas.Children.Remove(_connectionLine);
            _connectionLine = null;
        }
        public void AddArrowTip(Line arrowLine)
        {
            double arrowLength = 10; 
            double arrowWidth = 5;   

            double angle = Math.Atan2(
                arrowLine.Y2 - arrowLine.Y1,
                arrowLine.X2 - arrowLine.X1
            );

            var left = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = arrowLine.StrokeThickness,
                X1 = arrowLine.X2,
                Y1 = arrowLine.Y2,
                X2 = arrowLine.X2 - arrowLength * Math.Cos(angle - Math.PI / 6),
                Y2 = arrowLine.Y2 - arrowLength * Math.Sin(angle - Math.PI / 6)
            };

            var right = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = arrowLine.StrokeThickness,
                X1 = arrowLine.X2,
                Y1 = arrowLine.Y2,
                X2 = arrowLine.X2 - arrowLength * Math.Cos(angle + Math.PI / 6),
                Y2 = arrowLine.Y2 - arrowLength * Math.Sin(angle + Math.PI / 6)
            };

            AssociatedObject.Children.Add(left);
            AssociatedObject.Children.Add(right);
        }
    }
}
