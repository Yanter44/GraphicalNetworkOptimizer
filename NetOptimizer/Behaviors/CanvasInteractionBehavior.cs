using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models;
using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.ViewModels.MainWindoww;
using System.Diagnostics;
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
        public  Polyline _connectionLine;
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
                if (VM.CurrentTool != null)
                {
                    StartDrawing(e.GetPosition(AssociatedObject));
                    return;
                }
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
            if (VM.IsDrawing)
            {
                if(VM.CurrentTool.Type == UIToolElementType.Arrow)
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
                            X = (line.X1 + line.X2) / 2,
                            Y = (line.Y1 + line.Y2) / 2,
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
                            Points = new List<Point>(poly.Points)
                        };
                        break;

                    case TextBox tb:
                        creatableObject = new LabelElement
                        {
                            Type = UIToolElementType.Label,
                            X = Canvas.GetLeft(tb),
                            Y = Canvas.GetTop(tb),
                            Width = tb.Width,
                            Text = tb.Text
                        };
                        break;
                    
                }
                _drawElement = null;
                VM.IsDrawing = false;
                if (creatableObject != null)
                {
                    VM.UIObjects.Add(creatableObject);
                }
            }
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
