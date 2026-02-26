using NetOptimizer.Models;
using NetOptimizer.ViewModels;
using NetOptimizer.Views.DopViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetOptimizer.Views.MainWindow
{
    public partial class MainWindow
    {
        private Point _deviceClickPosition;
        private bool _isDraggingDevice = false;
        private FrameworkElement _activeElement = null;

        private List<(Line Line, DeviceOnCanvas Source, DeviceOnCanvas Target)> _visualLinks = new();
        private Line _connectionLine;

        private DeviceOnCanvas _sourceDeviceConnection;
        private object _sourcePortNumber;
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

            deviceContainer.ContextMenu = CreateDeviceContextMenu(deviceContainer);
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
                    string portName = p.PortNumber;

                    var portRow = new TextBlock { FontSize = 11, Margin = new Thickness(0, 1, 0, 1) };
                    portRow.Inlines.Add(new Run($"{portName}: ") { Foreground = Brushes.LightGray });

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
                if (hoverData.IsSelected != true)
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
            var currentElement = sender as FrameworkElement;
            var targetDevice = currentElement?.Tag as DeviceOnCanvas;
            if (_connectionLine != null && e.LeftButton == MouseButtonState.Pressed)
            {
                if (targetDevice != null && targetDevice != _sourceDeviceConnection)
                {
                    if (currentElement.ContextMenu != null)
                    {
                        currentElement.ContextMenu.PlacementTarget = currentElement;
                        currentElement.ContextMenu.IsOpen = true;
                    }
                }
                e.Handled = true;
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDraggingDevice = true;
                _activeElement = sender as FrameworkElement;
                _deviceClickPosition = e.GetPosition(_activeElement);
                _activeElement.CaptureMouse();

                e.Handled = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var element = sender as FrameworkElement;
                if (element?.ContextMenu != null)
                {
                    element.ContextMenu.PlacementTarget = element;
                    element.ContextMenu.IsOpen = true;
                }
                e.Handled = true;
            }
        }
        private ContextMenu CreateDeviceContextMenu(StackPanel container)
        {
            ContextMenu menu = new ContextMenu();
            var data = container.Tag as DeviceOnCanvas;
            MenuItem connectHeader = new MenuItem
            {
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("/Assets/Images/connect.png", UriKind.Relative)),
                    Width = 20,
                    Height = 20
                },
                Header = "Подключить"
            };
            if (data?.LogicDevice?.Ports != null)
            {
                foreach (var port in data.LogicDevice.Ports)
                {
                    string statusIcon = port.IsLinked ? "●" : "○";
                    string statusText = port.IsLinked ? "UP" : "DOWN";

                    MenuItem portItem = new MenuItem
                    {
                        Header = $"{statusIcon} {port.PortName} {port.PortNumber} ({statusText})",
                        Tag = port.PortNumber,
                        Foreground = port.IsLinked ? Brushes.LimeGreen : Brushes.Black
                    };

                    portItem.Click += (s, e) => {
                        var pNum = (s as MenuItem).Tag;
                        if (port.IsLinked)
                        {
                            _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>($"Ошибка: Порт {port.PortName} уже занят!");
                            return;
                        }

                        if (_connectionLine != null)
                        {

                            var sourcePort = _sourceDeviceConnection.LogicDevice.Ports
                                .FirstOrDefault(p => p.PortNumber.Equals(_sourcePortNumber));

                            if (sourcePort != null)
                            {
                                try
                                {
                                    sourcePort.LinkTo(port);
                                    CreateVisualConnection(_sourceDeviceConnection, data);
                                    RefreshDeviceMenu(_sourceDeviceConnection);
                                    RefreshDeviceMenu(data);
                                    if (this.DataContext is MainWindowViewModel vm)
                                    {
                                        vm.UpdateYamlFromCanvas();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>(ex.Message);
                                }
                            }

                            MainCanvas.Children.Remove(_connectionLine);
                            _connectionLine = null;
                            _sourceDeviceConnection = null;
                            _sourcePortNumber = null;
                        }
                        else
                        {
                            StartDrawConnectionLine(data, pNum);
                        }
                    };
                    connectHeader.Items.Add(portItem);
                }
            }
            menu.Items.Add(connectHeader);
            menu.Items.Add(new Separator());

            MenuItem deleteItem = new MenuItem { 
                Header = "Удалить",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("/Assets/Images/delete.png", UriKind.Relative)),
                    Width = 20,
                    Height = 20
                },
            };
            deleteItem.Click += (s, e) => 
            {
                RemoveDeviceFromCanvas(container);
            };
            menu.Items.Add(deleteItem);

            return menu;
        }
        private void RemoveDeviceFromCanvas(StackPanel container)
        {
            var deviceData = container.Tag as DeviceOnCanvas;
            if (deviceData == null) return;

            var relatedLinks = _visualLinks.Where(x => x.Source == deviceData || x.Target == deviceData).ToList();

            var neighborsToRefresh = new HashSet<DeviceOnCanvas>();

            foreach (var link in relatedLinks)
            {
                var neighbor = (link.Source == deviceData) ? link.Target : link.Source;
                neighborsToRefresh.Add(neighbor);

                MainCanvas.Children.Remove(link.Line);
                _visualLinks.Remove(link);
            }

            UnlinkAllPorts(deviceData);

            var vm = (MainWindowViewModel)this.DataContext;
            vm.DevicesOnCanvas.Remove(deviceData);
            MainCanvas.Children.Remove(container);

            foreach (var neighbor in neighborsToRefresh)
            {
                RefreshDeviceMenu(neighbor);
            }
            vm.UpdateYamlFromCanvas();
        }
        private void UnlinkAllPorts(DeviceOnCanvas device)
        {
            foreach (var port in device.LogicDevice.Ports)
            {
                if (port.IsLinked)
                {
                    port.Unlink();
                }
            }
        }
        private void RefreshDeviceMenu(DeviceOnCanvas device)
        {
            var visual = MainCanvas.Children.OfType<StackPanel>()
                         .FirstOrDefault(x => x.Tag == device);

            if (visual != null)
            {
                visual.ContextMenu = CreateDeviceContextMenu(visual);
            }
        }
        private void StartDrawConnectionLine(DeviceOnCanvas deviceOnCanvas, object pnumb)
        {
            _sourceDeviceConnection = deviceOnCanvas; // Сохраняем КТО тянет
            _sourcePortNumber = pnumb;
            var visualElement = MainCanvas.Children.OfType<StackPanel>()
                               .FirstOrDefault(enumerable => enumerable.Tag == deviceOnCanvas);
            if (visualElement != null)
            {

                double x = Canvas.GetLeft(visualElement);
                double y = Canvas.GetTop(visualElement);

                double centerX = x + (visualElement.ActualWidth / 2);
                double centerY = y + (visualElement.ActualHeight / 2);
                _connectionLine = new Line
                {
                    Stroke = Brushes.SlateGray,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection() { 6, 5 },
                    SnapsToDevicePixels = true,
                    X1 = centerX,
                    X2 = centerX, 
                    Y1 = centerY,
                    Y2 = centerY, 
                };
                MainCanvas.Children.Add(_connectionLine);
           
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
    }
}
