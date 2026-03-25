using Microsoft.Xaml.Behaviors;
using NetOptimizer.Models;
using NetOptimizer.ViewModels.MainWindow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NetOptimizer.Behaviors
{
    public class DeviceDragBehavior : Behavior<FrameworkElement>
    {
        private Point _clickPosition;
        private bool _isDragging;
        private FrameworkElement _activeElement;
        private Point _lastMousePosition;
        private DispatcherTimer _tooltipTimer;
        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseUp += OnMouseUp;
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;

            var canvasBehavior = GetCanvasBehavior();
            if (canvasBehavior != null)
                canvasBehavior.ConnectionCreated += OnConnectionCreated;

            var vm = GetMainVM();
            if (vm != null)
                vm.DevicesUpdated += OnDevicesUpdated;
        }
        private void OnDevicesUpdated(DeviceOnCanvas[] devices)
        {
            if (AssociatedObject.DataContext is DeviceOnCanvas device)
            {
                if (devices.Contains(device))
                {
                    AssociatedObject.ContextMenu = null;
                    AssociatedObject.ContextMenu = CreateDeviceContextMenu(AssociatedObject);
                }
            }
        }
        private void OnConnectionCreated(DeviceOnCanvas source, DeviceOnCanvas target)
        {
            if (AssociatedObject.DataContext is DeviceOnCanvas device)
            {
                if (device == source || device == target)
                {
                    AssociatedObject.ContextMenu = null;
                    AssociatedObject.ContextMenu = CreateDeviceContextMenu(AssociatedObject);
                }
            }
        }
        private MainWindowViewModel GetMainVM()
        {
            DependencyObject parent = AssociatedObject;
            while (parent != null)
            {
                if (parent is FrameworkElement fe && fe.DataContext is MainWindowViewModel vm)
                    return vm;

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var device = AssociatedObject.DataContext as DeviceOnCanvas;
            var vm = GetMainVM();
            if (device != null && vm != null)
            {
                vm.SelectDevice(device, true); 
            }
            if(_tooltipTimer == null)
            {
                _tooltipTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                _tooltipTimer.Start();
                _tooltipTimer.Tick += (s, args) =>
                {
                    _tooltipTimer.Stop();
                    ShowDeviceToolTip(device);
                };
            }
             _tooltipTimer.Stop();  
             _tooltipTimer.Start();         
        }

        
        private void ShowDeviceToolTip(DeviceOnCanvas device)
        {
            if (AssociatedObject.ToolTip == null)
            {
                AssociatedObject.ToolTip = new ToolTip
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
            header.Inlines.Add(new Run(device.DeviceName.ToUpper())
            {
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            });

            var portsStack = new StackPanel();
            portsStack.Children.Add(header);
            portsStack.Children.Add(new Separator
            {
                Background = Brushes.DimGray,
                Margin = new Thickness(0, 2, 0, 5)
            });

            foreach (var p in device.LogicDevice.Ports)
            {
                var portRow = new TextBlock { FontSize = 11, Margin = new Thickness(0, 1, 0, 1) };
                portRow.Inlines.Add(new Run($"{p.PortNumber}: ") { Foreground = Brushes.LightGray });

                if (p.IsLinked)
                {
                    string directionArrow = p.IsInitiator ? " → " : " ← ";
                    portRow.Inlines.Add(new Run("✔ LINK") { Foreground = Brushes.LimeGreen, FontWeight = FontWeights.SemiBold });
                    portRow.Inlines.Add(new Run(directionArrow) { Foreground = Brushes.Orange, FontWeight = FontWeights.Bold });
                    portRow.Inlines.Add(new Run($"{p.ConnectedTo.Owner.Name} [{p.ConnectedTo.PortNumber}]")
                    {
                        Foreground = Brushes.Gray,
                        FontSize = 10
                    });
                }
                else
                {
                    portRow.Inlines.Add(new Run("❌ DOWN") { Foreground = Brushes.IndianRed });
                }

                portsStack.Children.Add(portRow);
            }

            var tooltipContent = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(245, 10, 10, 10)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(10),
                Child = portsStack
            };

            if (AssociatedObject.ToolTip is ToolTip tt)
                tt.Content = tooltipContent;

            (AssociatedObject.ToolTip as ToolTip)?.IsOpen = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var device = AssociatedObject.DataContext as DeviceOnCanvas;
            var vm = GetMainVM();
            if (device != null && vm != null)
            {
                vm.SelectDevice(device, false);
            }
            if (AssociatedObject.ToolTip is ToolTip tt)
                tt.IsOpen = false;

            _tooltipTimer?.Stop();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvasBehavior = GetCanvasBehavior();

            if (canvasBehavior?.IsTryingToConnect == true)
            {
                var element = sender as FrameworkElement;

                if (element != null)
                {
                    if (element.ContextMenu == null)
                        element.ContextMenu = CreateDeviceContextMenu(element);

                    element.ContextMenu.PlacementTarget = element;
                    element.ContextMenu.IsOpen = true;
                }
                e.Handled = true;
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                _isDragging = true;
                _activeElement = sender as FrameworkElement;
                _clickPosition = e.GetPosition(_activeElement);
                _lastMousePosition = e.GetPosition(_activeElement.Parent as Canvas);
                _activeElement.CaptureMouse();
                e.Handled = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var element = sender as FrameworkElement;
                if (element != null)
                {
                    if (element.ContextMenu == null)
                        element.ContextMenu = CreateDeviceContextMenu(element);

                    element.ContextMenu.PlacementTarget = element;
                    element.ContextMenu.IsOpen = true;
                }
                e.Handled = true;
            }
        }
        private CanvasInteractionBehavior GetCanvasBehavior()
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window == null) return null;

            var canvas = window.FindName("MainCanvas") as Canvas;
            if (canvas == null) return null;

            return Interaction.GetBehaviors(canvas)
                              .OfType<CanvasInteractionBehavior>()
                              .FirstOrDefault();
        }
        private ContextMenu CreateDeviceContextMenu(FrameworkElement element)
        {
            var menu = new ContextMenu();
            if (!(element.DataContext is DeviceOnCanvas device))
                return menu;

            var connectHeader = new MenuItem { Header = "Подключить", Icon = CreateIcon("/Assets/Images/connect.png") };
            foreach (var port in device.LogicDevice.Ports)
            {
                var portItem = CreatePortMenuItem(port);
                portItem.Click += (s, e) =>
                {
                    var vm = GetMainVM();
                    var canvasBehavior = GetCanvasBehavior();

                    if (port.IsLinked)
                    {
                        vm?.ShowPortError(port);
                        return;
                    }
                    if (canvasBehavior.IsTryingToConnect)
                    {
                        canvasBehavior.FinishConnection(device, port);
                    }
                    else
                    {
                        StartDrawConnectionLine(device, port);
                    }
                };
                connectHeader.Items.Add(portItem);
            }
            menu.Items.Add(connectHeader);
            menu.Items.Add(new Separator());

            var deleteItem = new MenuItem { Header = "Удалить", Icon = CreateIcon("/Assets/Images/delete.png") };
            deleteItem.Click += (s, e) =>
            {
                var vm = GetMainVM();
                vm?.RemoveDevice(device);
            };
            menu.Items.Add(deleteItem);

            return menu;
        }
        private void StartDrawConnectionLine(DeviceOnCanvas sourceDevice, Port sourceport)
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window == null) return;

            var  _connectionLine = window.FindName("TempLine") as Polyline;
            if (_connectionLine == null) return;

            Point startPoint = new Point(sourceDevice.X + 32, sourceDevice.Y + 32);

            var canvas = window.FindName("MainCanvas") as Canvas; 
            if (canvas == null) return;

            var canvasBehavior = Interaction.GetBehaviors(canvas)
                                            .OfType<CanvasInteractionBehavior>()
                                            .FirstOrDefault();
            if (canvasBehavior != null)
            {
                canvasBehavior.StartConnectionLine(startPoint, _connectionLine, sourceDevice, sourceport);
            }
        }
        private MenuItem CreatePortMenuItem(Port port) => new MenuItem
        {
            Header = $"{(port.IsLinked ? "●" : "○")} {port.PortName} {port.PortNumber} ({(port.IsLinked ? "Linked" : "Not Linked")})",
            Tag = port.PortNumber,
            Foreground = port.IsLinked ? Brushes.LimeGreen : Brushes.Black
        };
        private Image CreateIcon(string path) => new Image
        {
            Source = new BitmapImage(new Uri(path, UriKind.Relative)),
            Width = 20,
            Height = 20
        };

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _activeElement == null) return;

            var device = _activeElement.DataContext as DeviceOnCanvas;
            var vm = GetMainVM();
            if (device == null || vm == null) return;

            Point currentMouse = e.GetPosition(_activeElement.Parent as Canvas);

            double dx = currentMouse.X - _lastMousePosition.X;
            double dy = currentMouse.Y - _lastMousePosition.Y;

            double adjustedDx = dx / vm.CanvasVM.CanvasScale;
            double adjustedDy = dy / vm.CanvasVM.CanvasScale;

            if (device.IsSelected)
            {
                foreach (var selected in vm.DevicesOnCanvas.Where(d => d.IsSelected))
                    vm.MoveDevice(selected, adjustedDx, adjustedDy);
            }
            else
            {
                vm.MoveDevice(device, adjustedDx, adjustedDy);
            }
            _lastMousePosition = currentMouse;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _activeElement?.ReleaseMouseCapture();
            _activeElement = null;
        }
    }
}
