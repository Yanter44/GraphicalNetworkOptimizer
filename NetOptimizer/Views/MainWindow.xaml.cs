using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using NetOptimizer.Models;
using NetOptimizer.ViewModels;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace NetOptimizer
{
    public partial class MainWindow : Window
    {
        private Point _selectionStartPoint;
        private Point _lastMousePosition;
        private Point _deviceClickPosition;

        private bool _isPanning = false;

        private bool _isDraggingDevice = false;
        private FrameworkElement _activeElement = null;

        private List<(Line Line, DeviceOnCanvas Source, DeviceOnCanvas Target)> _visualLinks = new();
        public MainWindow(MainWindowViewModel viewmodel)
        {
            InitializeComponent();
            this.DataContext = viewmodel;

            viewmodel.DeviceAdded += OnDeviceAddedToCanvas;
            viewmodel.ConnectionRequested += CreateVisualConnection;
            LoadCustomYamlHighlighting();

            this.Loaded += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    YamlEditor.Text = vm.CurrentYamlText;
                    YamlEditor.TextChanged += (sender, args) =>
                    {
                        vm.CurrentYamlText = YamlEditor.Text;
                    };
                    vm.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(vm.CurrentYamlText))
                        {
                            if (YamlEditor.Text != vm.CurrentYamlText)
                                YamlEditor.Text = vm.CurrentYamlText;
                        }
                    };
                }
            };
        }
        private void LoadCustomYamlHighlighting()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith("MyYmlSyntax.xshd"));

            if (string.IsNullOrEmpty(resourceName))
            {
                MessageBox.Show("ОШИБКА: Файл .xshd не найден в ресурсах сборки!");
                foreach (var name in assembly.GetManifestResourceNames())
                    MessageBox.Show($"Доступный ресурс: {name}");
                return;
            }

            using (Stream s = assembly.GetManifestResourceStream(resourceName))
            {
                if (s == null) return;
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    YamlEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        private void OnDeviceAddedToCanvas(object sender, DeviceOnCanvas data)
        {
            StackPanel deviceContainer = new StackPanel
            {
                Width = 64,
                Cursor = Cursors.Hand,
                Background = Brushes.Transparent,
                Tag = data 
            };
            deviceContainer.MouseEnter += ShowMyToolTip;
            deviceContainer.MouseDown += Device_MouseDown;
            deviceContainer.MouseMove += Device_MouseMove;
            deviceContainer.MouseUp += Device_MouseUp;
            deviceContainer.MouseEnter += Device_MouseEnter;
            deviceContainer.MouseLeave += Device_MouseLeave;

            var myUri = new Uri($"pack://application:,,,/{data.ImagePath}", UriKind.Absolute);
            Image deviceImg = new Image { Source = new BitmapImage(myUri), Width = 48, Height = 48 };
            TextBlock deviceText = new TextBlock { Text = data.DeviceName, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 10 };

            deviceContainer.Children.Add(deviceImg);
            deviceContainer.Children.Add(deviceText);

            Canvas.SetLeft(deviceContainer, data.X);
            Canvas.SetTop(deviceContainer, data.Y);

            MainCanvas.Children.Add(deviceContainer);
        }
        private void ShowMyToolTip(object sender, MouseEventArgs e)
        {
            if (sender is StackPanel panel && panel.Tag is DeviceOnCanvas data)
            {
                if (panel.ToolTip == null)
                {
                    panel.ToolTip = new ToolTip
                    {
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(0)
                    };
                }

                var header = new TextBlock
                {
                    Margin = new Thickness(0, 0, 0, 5),
                    FontSize = 12
                };

                header.Inlines.Add(new Run($"{data.DeviceName.ToUpper()}")
                {
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold
                });

                var portsStack = new StackPanel();
                portsStack.Children.Add(header);
                portsStack.Children.Add(new Separator { Background = Brushes.DimGray, Margin = new Thickness(0, 2, 0, 5) });
                for (int i = 0; i < data.LogicDevice.Ports.Count; i++)
                {
                    var p = data.LogicDevice.Ports[i];
                    string portName = $"{i / 4}/{i % 4}";

                    var portRow = new TextBlock { FontSize = 11 };
                    portRow.Inlines.Add(new Run($"{portName}: ") { Foreground = Brushes.LightGray });

                    if (p.IsLinked)
                    {
                        portRow.Inlines.Add(new Run("✔ LINK UP") { Foreground = Brushes.LimeGreen, FontWeight = FontWeights.SemiBold });
                        portRow.Inlines.Add(new Run($" (to {p.ConnectedTo.Owner.Name})") { Foreground = Brushes.Gray, FontSize = 10 });
                    }
                    else
                    {
                        portRow.Inlines.Add(new Run("❌ DOWN") { Foreground = Brushes.IndianRed });
                    }

                    portsStack.Children.Add(portRow);
                }

                var cyberOrangeContainer = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(245, 10, 10, 10)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(10),
                    Child = portsStack
                };

                if (panel.ToolTip is ToolTip tt)
                {
                    tt.Content = cyberOrangeContainer;
                }
            }
        }
        private void CreateVisualConnection(DeviceOnCanvas source, DeviceOnCanvas target)
        {
            var sourcePanel = MainCanvas.Children.OfType<StackPanel>().FirstOrDefault(sp => sp.Tag == source);
            var targetPanel = MainCanvas.Children.OfType<StackPanel>().FirstOrDefault(sp => sp.Tag == target);

            if (sourcePanel != null && targetPanel != null)
            {
                var sourcePanelX = Canvas.GetLeft(sourcePanel);
                var sourcePanelY = Canvas.GetTop(sourcePanel);
                
                var targetPanelX = Canvas.GetLeft(targetPanel);
                var targetPanelY = Canvas.GetTop(targetPanel);

                Line connectionLine = new Line
                {
                    Stroke = Brushes.SlateGray,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection() { 6, 5 },
                    SnapsToDevicePixels = true
                };
          
                MainCanvas.Children.Add(connectionLine);
                var newLink = (connectionLine, source, target);
                _visualLinks.Add(newLink);

                var pairLinks = _visualLinks.Where(l =>
                    (l.Source == source && l.Target == target) ||
                    (l.Source == target && l.Target == source));

                foreach (var link in pairLinks)
                {
                    UpdateLineGeometry(link);
                }

            }
        }
        private void UpdateLineGeometry((Line Line, DeviceOnCanvas Source, DeviceOnCanvas Target) link)
        {
            double sourceX = link.Source.X + 32;
            double sourceY = link.Source.Y + 32;
            double targetX = link.Target.X + 32;
            double targetY = link.Target.Y + 32;

            double dx = targetX - sourceX;
            double dy = targetY - sourceY;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance > 0)
            {
                var pairLinks = _visualLinks.Where(l =>
                    (l.Source == link.Source && l.Target == link.Target) ||
                    (l.Source == link.Target && l.Target == link.Source)).ToList();


                int indexInPair = pairLinks.IndexOf(link);
                double offsetAmount = (indexInPair - (pairLinks.Count - 1) / 2.0) * 5;

                double nx = -dy / distance;
                double ny = dx / distance;

                link.Line.X1 = sourceX + nx * offsetAmount;
                link.Line.Y1 = sourceY + ny * offsetAmount;
                link.Line.X2 = targetX + nx * offsetAmount;
                link.Line.Y2 = targetY + ny * offsetAmount;
            }
        }
        private void Device_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is StackPanel panel && panel.Tag is DeviceOnCanvas hoverData)
            {
                panel.Background = new SolidColorBrush(Color.FromArgb(50, 0, 120, 215));  
            }
        }
        private void Device_MouseLeave(object sender, MouseEventArgs e) 
        {
            if (sender is StackPanel panel && panel.Tag is DeviceOnCanvas hoverData)
            {
                if(hoverData.IsSelected != true)
                {
                   panel.Background = Brushes.Transparent;
                }
            }
        }
        private void Device_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingDevice && _activeElement != null)
            {
                Point currentPos = e.GetPosition(MainCanvas);
                var activeData = _activeElement.Tag as DeviceOnCanvas;

                if (activeData != null)
                {
                    double newX = currentPos.X - _deviceClickPosition.X;
                    double newY = currentPos.Y - _deviceClickPosition.Y;

                    double deltaX = newX - activeData.X;
                    double deltaY = newY - activeData.Y;

                    var vm = (MainWindowViewModel)this.DataContext;
                    var selectedDevices = vm.DevicesOnCanvas.Where(d => d.IsSelected).ToList();

                    if (activeData.IsSelected)
                    {
                        foreach (var device in selectedDevices)
                        {
                            MoveDevice(device, deltaX, deltaY);
                        }
                    }
                    else
                    {
                        MoveDevice(activeData, deltaX, deltaY);
                    }
                }
            }
        }

        private void MoveDevice(DeviceOnCanvas device, double dx, double dy)
        {
            device.X += dx;
            device.Y += dy;

            var visual = MainCanvas.Children.OfType<FrameworkElement>()
                         .FirstOrDefault(x => x.Tag == device);

            if (visual != null)
            {
                Canvas.SetLeft(visual, device.X);
                Canvas.SetTop(visual, device.Y);

                var relatedLinks = _visualLinks.Where(l => l.Source == device || l.Target == device);
                foreach (var link in relatedLinks)
                {
                    UpdateLineGeometry(link);
                }
            }
        }
        private void Device_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDraggingDevice = true;
                _activeElement = sender as FrameworkElement;
                _deviceClickPosition = e.GetPosition(_activeElement); 
                _activeElement.CaptureMouse();
                
                e.Handled = true; 
            }
        }

      
        private void Device_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingDevice = false;
            if (_activeElement != null)
            {
                _activeElement.ReleaseMouseCapture();
                _activeElement = null;
            }
        }
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
            if (_isPanning)
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