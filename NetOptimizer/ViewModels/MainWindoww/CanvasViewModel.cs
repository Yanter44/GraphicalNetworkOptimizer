using MediatR;
using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Events;
using NetOptimizer.Interfaces;
using NetOptimizer.MediatR.Commands;
using NetOptimizer.Models;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using NetOptimizer.Services;
using NetOptimizer.ViewModels.DeviceParametrs;
using NetOptimizer.ViewModels.DeviceSettingss;
using NetOptimizer.ViewModels.MainWindow;
using NetOptimizer.Views;
using NetOptimizer.Views.DopViews;
using Newtonsoft.Json.Bson;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;

namespace NetOptimizer.ViewModels.MainWindoww
{
    public class CanvasViewModel : INotifyPropertyChanged
    {
        private const int MaxConnectionsBetweenDevices = 6;

        private Point _canvasOffset;
        private double _canvasScale = 1;

        private Rect _selectionRect;
        private bool _isSelecting;
        private bool _isDrawSelection;

        private double _canvasWidth;
        private double _canvasHeight;
        private UIToolElementToAddDto _currentTool;
        private bool _isDrawing;
        private bool _isTryingToConnect;
        private TempLine _tempConnectionLine;
        private bool _isTryingToSendPacket;

        private DeviceOnCanvas _sourceDevice;
        private Port _sourcePort;
        public double CanvasWidth { get => _canvasWidth; set { _canvasWidth = value; OnPropertyChanged(); } }
        public double CanvasHeight { get => _canvasHeight; set { _canvasHeight = value; OnPropertyChanged(); } }
        public Point CanvasOffset { get => _canvasOffset; set { _canvasOffset = value; OnPropertyChanged(); } }
        public double CanvasScale { get => _canvasScale; set { _canvasScale = value; OnPropertyChanged(); }}
        public Rect SelectionRect { get => _selectionRect; set { _selectionRect = value; OnPropertyChanged(); } }
        public bool IsDrawingSelection { get => _isDrawSelection; set { _isDrawSelection = value; OnPropertyChanged(); } }
        public bool IsSelecting { get => _isSelecting; set { _isSelecting = value; OnPropertyChanged(); } }
        public UIToolElementToAddDto CurrentTool { get => _currentTool; set { _currentTool = value;  OnPropertyChanged(); }}
        public bool IsDrawing { get => _isDrawing; set { _isDrawing = value; OnPropertyChanged(); UpdateCanvasCursor(); } }
        public bool IsTryingToSendPacket { get => _isTryingToSendPacket; set { _isTryingToSendPacket = value; OnPropertyChanged(); UpdateCanvasCursor(); }  }
        public bool IsTryingToConnect { get => _isTryingToConnect; set { _isTryingToConnect = value; OnPropertyChanged(); UpdateCanvasCursor(); } }
        public TempLine TempConnectionLine { get => _tempConnectionLine; set { _tempConnectionLine = value; OnPropertyChanged(); } }
        public CanvasCursorMode CanvasCursor => ResolveCursor();
        public ObservableCollection<DeviceConnection> Connections { get; } = new ObservableCollection<DeviceConnection>();
        public ObservableCollection<DeviceOnCanvas> Devices { get; set; }  = new ObservableCollection<DeviceOnCanvas>();
        public ObservableCollection<UIElementBase> UIObjects { get; set; } = new();
        public ObservableCollection<PacketViewModel> UIPackets { get; set; } = new();
        public ObservableCollection<PacketTrailDotViewModel> UIPacketTrails { get; set; } = new();

        private CanvasClipBoard _clipboard = new();

        public event Action<DeviceOnCanvas[]> DevicesUpdated;

        public event EventHandler<DeviceOnCanvas> DeviceAdded;

        public event Action<DeviceOnCanvas, DeviceOnCanvas> ConnectionRequested;

        private readonly IWindowNavigator _windowNavigator;
        private readonly IMediator _mediator;
        private readonly ISimulationEngine _engine;
        private readonly IDeviceRegistry _deviceRegistry;
        public ICommand DeleteDeviceCommand { get; }
        public ICommand SelectDevicePortCommand { get; }
        public ICommand MoveDeviceCommand { get; }
        public ICommand DeviceMouseDownCommand { get; }
        public ICommand DeviceMouseOverCommand { get; }
        public ICommand StartTryToSendPacketCommand { get; }
        public CanvasViewModel(IWindowNavigator windowNavigator, IMediator mediator, ISimulationEngine engine, IDeviceRegistry deviceRegistry)
        {
            DeleteDeviceCommand = new RelayCommand(obj => DeleteDevice(obj));
            SelectDevicePortCommand = new RelayCommand(obj => SelectDevicePort(obj));
            StartTryToSendPacketCommand = new RelayCommand(obj => StartTryToSendPacket(obj));
            DeviceMouseDownCommand = new AsyncRelayCommand(obj => OnDeviceMouseDown(obj));
            DeviceMouseOverCommand = new RelayCommand(obj => OnDeviceMouseOver(obj));
            MoveDeviceCommand = new RelayCommand(obj => OnMoveDevice(obj));
            _windowNavigator = windowNavigator;
            _mediator = mediator;
            _engine = engine;
            _deviceRegistry = deviceRegistry;
            _engine.EventChain.CollectionChanged += OnSimulationEvent;
        }
        private void OnSimulationEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            foreach (SimmulationEvent ev in e.NewItems)
            {
                var from = Devices.FirstOrDefault(d => d.LogicDevice.Id == ev.FromDeviceId);
                var to = Devices.FirstOrDefault(d => d.LogicDevice.Id == ev.ToDeviceId);

                if (from == null || to == null)
                    continue;

                var packet = new PacketViewModel
                {
                    PacketId = ev.PacketId,
                    FromDeviceId = ev.FromDeviceId,
                    ToDeviceId = ev.ToDeviceId,
                    FromDevice = from,
                    ToDevice = to,
                    Progress = 0
                };

                UIPackets.Add(packet);
                _ = AnimatePacket(packet);
            }
        }
        private async Task AnimatePacket(PacketViewModel packet)
        {
            int steps = 20;

            for (int i = 0; i <= steps; i++)
            {
                packet.Progress = i / (double)steps;

                UIPacketTrails.Add(new PacketTrailDotViewModel
                {
                    PacketId = packet.PacketId,
                    X = packet.X,
                    Y = packet.Y,
                });
                await Task.Delay(50);
            }
            UIPackets.Remove(packet);
            var toRemove = UIPacketTrails.Where(t => t.PacketId == packet.PacketId)
                                         .ToList();

            foreach (var dot in toRemove)
                UIPacketTrails.Remove(dot);
        }

        private DeviceConnection FindConnection(string fromId, string toId)
        {
            return Connections.FirstOrDefault(c =>
                (c.Source.LogicDevice.Id == fromId && c.Target.LogicDevice.Id == toId) ||
                (c.Source.LogicDevice.Id == toId && c.Target.LogicDevice.Id == fromId));
        }
        public void OnDeviceCreated(DeviceToAddDto dto, DeviceSettingsBase settings, double x = 500, double y = 500)
        {
            Device logicDevice = (dto.Type, settings) switch
            {
                (DeviceType.PC, PcSettingsViewModel p) => new PcDevice(p.Name, p.GetSettings()),
                (DeviceType.Switch, SwitchSettingViewModel s) => new SwitchDevice(s.Name, s.GetSettings()),
                (DeviceType.Router, RouterSettingsViewModel r) => new RouterDevice(r.Name, r.GetSettings()),
                _ => null
            };
            if (logicDevice == null) return;
            var deviceOnCanvas = new DeviceOnCanvas(logicDevice, x, y);
            Devices.Add(deviceOnCanvas);
            _deviceRegistry.Register(deviceOnCanvas);
            // UpdateYamlFromCanvas();
        }
        private void OnDeviceMouseOver(object obj)
        {
            var args = obj as DeviceMouseEventArgs;
            if (args?.Device == null) return;

            switch (args.Action)
            {
                case DeviceMouseAction.MouseEnter:
                    args.Device.IsSelected = true;
                    break;

                case DeviceMouseAction.MouseLeave:
                    if (!IsSelecting) 
                        args.Device.IsSelected = false;
                    break;
            }
        }
        private void OnMoveDevice(object obj)
        {
            if (obj is not DeviceMoveEventArgs args)
                return;

            var selectedDevices = Devices
                .Where(x => x.IsSelected)
                .ToList();

            if (selectedDevices.Count > 1)
            {
                foreach (var dev in selectedDevices)
                {
                    MoveDevice(dev, args.Dx, args.Dy);
                }
            }
            else
            {
                MoveDevice(args.Device, args.Dx, args.Dy);
            }
        }
        private async Task OnDeviceMouseDown(object obj)
        {
            var args = obj as DeviceMouseEventArgs;

            switch (args.Action)
            {
                case DeviceMouseAction.DoubleClick:
                    ShowDeviceParametrsWindow(args.Device);
                    break;

                case DeviceMouseAction.SingleClick:
                    if (IsTryingToSendPacket && _sourceDevice != null)
                    {
                        var target = args.Device;
                        if (target == _sourceDevice) return;
                        await _mediator.Send(new SendPacketCommand()
                        {
                            SourceId = _sourceDevice.LogicDevice.Id,
                            TargetId = target.LogicDevice.Id
                        });
                        _sourceDevice = null;
                        IsTryingToSendPacket = false;
                    }
                    break;
            }
        }
        private void UpdateCanvasCursor()
        {
            OnPropertyChanged(nameof(CanvasCursor));
        }

        public CanvasCursorMode ResolveCursor()
        { 
            if (IsTryingToSendPacket)
                 return CanvasCursorMode.SendPacket;

            if (IsTryingToConnect)
                 return CanvasCursorMode.Connect;

            if (IsDrawing)
                 return CanvasCursorMode.Draw;

           return CanvasCursorMode.Default;
        }

        private void StartTryToSendPacket(object obj)
        {
            if (obj is not DeviceOnCanvas device)
                return;
            _sourceDevice = device;
            IsTryingToSendPacket = true;
        }


        public void SelectDevicePort(object obj)
        {
            var port = obj as Port;
            var deviceOnCanvas = Devices.FirstOrDefault(deviceOnCanvas => deviceOnCanvas.LogicDevice == port.Owner);
            if (port.IsLinked)
            {
                ShowPortError(port);
                return;
            }
            if (IsTryingToConnect)
            {
                if(deviceOnCanvas != null)
                {
                    FinishConnection(deviceOnCanvas, port);
                }  
            }
            else
            {
                StartDrawConnectionLine(deviceOnCanvas, port);
            }
        }
        public void FinishConnection(DeviceOnCanvas targetDevice, Port targetPort)
        {
            if (TryConnectPorts(_sourceDevice, _sourcePort, targetDevice, targetPort))
            {
                StopConnectionLine();
            }
        }
        public void StopConnectionLine()
        {
            if (TempConnectionLine == null) return;
            IsTryingToConnect = false;
            TempConnectionLine = null;
            _sourceDevice = null;
        }
        public void CopySelected()
        {
            _clipboard = new CanvasClipBoard();

            var selectedDevices = Devices.Where(d => d.IsSelected).ToList();
            var selectedUI = UIObjects.Where(u => u.IsSelected).ToList();

            foreach (var device in selectedDevices)
            {
                var modelCopiedDevice = new DeviceOnCanvas(device.LogicDevice, device.X, device.Y)
                {
                    LogicDevice = device.LogicDevice,
                    IsSelected = false
                };
                _clipboard.Devices.Add(modelCopiedDevice);
            }

            foreach (var ui in selectedUI)
            {
           //     var uielement = new UiEle
            //    _clipboard.UIElements.Add(ui);
            }

            foreach (var conn in Connections)
            {
                if (selectedDevices.Contains(conn.Source) &&
                    selectedDevices.Contains(conn.Target))
                {
                    _clipboard.Connections.Add(conn);
                }
            }
        }
        private void StartDrawConnectionLine(DeviceOnCanvas sourceDevice, Port sourceport)
        {
            IsTryingToConnect = true;
            _sourceDevice = sourceDevice;
            _sourcePort = sourceport;
            TempConnectionLine = new TempLine
            {
                Start = new Point(sourceDevice.X + 32, sourceDevice.Y + 32),
                End = new Point(sourceDevice.X + 32, sourceDevice.Y + 32)
            };
        }
        public void DeleteDevice(object obj)
        {
            var device = obj as DeviceOnCanvas;
            var connectionsToRemove = Connections.Where(c => c.Source == device || c.Target == device).ToList();
                var neighbors = connectionsToRemove
                                .SelectMany(c => new[] { c.Source, c.Target })
                                .Where(d => d != device)
                                .Distinct()
                                .ToArray();

                foreach (var conn in connectionsToRemove)
                {
                    conn.SourcePort?.Unlink();
                    conn.TargetPort?.Unlink();
                    Connections.Remove(conn);
                }

                Devices.Remove(device);
                //UpdateYamlFromCanvas();
                DevicesUpdated?.Invoke(neighbors);
        }
        public void DrawAllConnectionsForDevice(DeviceOnCanvas deviceOnCanvas)
        {
            foreach (var port in deviceOnCanvas.LogicDevice.Ports)
            {
                if (port.IsLinked)
                {
                    var remotePort = port.ConnectedTo;
                    var targetVisual = Devices.FirstOrDefault(d => d.LogicDevice == remotePort.Owner);

                    if (targetVisual != null)
                    {
                        ConnectionRequested?.Invoke(deviceOnCanvas, targetVisual);
                    }
                }
            }
        }
        public void ShowPortError(Port port) => _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>($"Ошибка: Порт {port.PortName} уже занят!");
        public void ShowDeviceParametrsWindow(DeviceOnCanvas device) => _windowNavigator.ShowModalView<DeviceParametrsWindow, DeviceParametrsViewModel>(device);
        
        public void MoveDevice(DeviceOnCanvas device, double dx, double dy)
        {
            var target = Devices.FirstOrDefault(d => d == device);
            if (target == null) return;

            target.X += dx;
            target.Y += dy;
        }
        public void MoveUIElement(UIElementBase element, Vector delta)
        {
            if (element == null) return;

            element.X += delta.X;
            element.Y += delta.Y;

            switch (element)
            {
                case LineElement line:
                    line.Start = new Point(line.Start.X + delta.X, line.Start.Y + delta.Y);
                    line.End = new Point(line.End.X + delta.X, line.End.Y + delta.Y);
                    break;

                case CurveElement curve:
                    if (curve.Points == null || curve.Points.Count == 0)
                        return;
                    var newPoints = new PointCollection();

                    foreach (var p in curve.Points)
                    {
                        newPoints.Add(new Point(p.X + delta.X, p.Y + delta.Y));
                    }

                    curve.Points = newPoints;
                    break;

                case EllipseElement:
                case LabelElement:
                    break;
            }
        }
        public bool TryConnectPorts(DeviceOnCanvas sourceDevice, Port sourcePort,
                                    DeviceOnCanvas targetDevice, Port targetPort)
        {
            if (targetPort.IsLinked)
                return ShowError($"Порт {targetPort.PortName} уже занят!");

            if (sourcePort.Type != targetPort.Type)
                return ShowError($"Нельзя соединить {sourcePort.Type} с {targetPort.Type}");

            try
            {
                sourcePort.LinkTo(targetPort);
                var conn = new DeviceConnection
                {
                    Source = sourceDevice,
                    Target = targetDevice,
                    SourcePort = sourcePort,
                    TargetPort = targetPort
                };

                Connections.Add(conn);

                var group = GetConnectionsBetween(sourceDevice, targetDevice);

                for (int i = 0; i < group.Count; i++)
                {
                    group[i].ConnectionIndex = i;
                }

                foreach (var c in group)
                {
                    if (c.ConnectionIndex < MaxConnectionsBetweenDevices)
                        c.UpdateLine();
                }
                return true;
            }
            catch (Exception ex)
            {
                return ShowError(ex.Message);
            }
            bool ShowError(string message)
            {
                _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>(message);
                return false;
            }
        }
        public List<DeviceConnection> GetConnectionsBetween(DeviceOnCanvas a, DeviceOnCanvas b)
        {
            return Connections
                .Where(c =>
                    (c.Source == a && c.Target == b) ||
                    (c.Source == b && c.Target == a))
                .ToList();
        }
        public void SelectDevice(DeviceOnCanvas device, bool isSelected)
        {
            if (device != null)
                device.IsSelected = isSelected;
        }
        public void SelectUiObject(UIElementBase uiobject, bool isSelected)
        {
            if(uiobject != null)
                uiobject.IsSelected = isSelected;
        }    
        public void DeleteUiObject(UIElementBase uiObject)
        {
            UIObjects.Remove(uiObject);
        }
        public void DeleteSelected()
        {
            var uiObjects = UIObjects.Where(x => x.IsSelected).ToList();
            var devices = Devices.Where(x => x.IsSelected).ToList();

            foreach (var obj in uiObjects)
                DeleteUiObject(obj);

            foreach (var device in devices)
                DeleteDevice(device);
        }
        public void SetCurrentTool(UIToolElementToAddDto tool)
        {
            CurrentTool = tool;
        }
        public void Zoom(double delta)
        {
            double zoomSpeed = 0.1;
            double newScale = CanvasScale + delta * zoomSpeed;

            if (newScale >= 0.3 && newScale <= 3)
                CanvasScale = newScale;
        }

        public void Pan(Vector delta)
        {
            CanvasOffset = new Point(
                CanvasOffset.X + delta.X,
                CanvasOffset.Y + delta.Y);
        }

        public void StartSelection(Point start)
        {
            SelectionRect = new Rect(start, new Size(0, 0));
            IsSelecting = true;
            IsDrawingSelection = true;
        }

        public void UpdateSelection(Point start, Point current)
        {
            double x = Math.Min(start.X, current.X);
            double y = Math.Min(start.Y, current.Y);

            double w = Math.Abs(current.X - start.X);
            double h = Math.Abs(current.Y - start.Y);

            SelectionRect = new Rect(x, y, w, h);

            foreach (var device in Devices)
            {
                Rect deviceRect = new Rect(device.X, device.Y, 64, 64);
                device.IsSelected = SelectionRect.IntersectsWith(deviceRect);
            }

            foreach (var uiObject in UIObjects)
            {
                if (uiObject is LineElement line)
                {
                    uiObject.IsSelected = LineNearRect(line.Start, line.End, SelectionRect);
                }
                else if (uiObject is ArrowElement arrow)
                {
                    uiObject.IsSelected =
                        LineNearRect(arrow.Start, arrow.End, SelectionRect) || 
                        LineNearRect(arrow.End, arrow.Tip1, SelectionRect) ||    
                        LineNearRect(arrow.End, arrow.Tip2, SelectionRect);    
                }
                else if (uiObject is RectangleElement rect)
                {
                    Rect r = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
                    uiObject.IsSelected = SelectionRect.IntersectsWith(r);
                }
                else if (uiObject is EllipseElement ellipse)
                {
                    Rect r = new Rect(ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height);
                    uiObject.IsSelected = SelectionRect.IntersectsWith(r);
                }
                else if (uiObject is LabelElement label)
                {
                    Rect r = new Rect(label.X, label.Y, label.Width, 20); // высоту можно нормальную потом
                    uiObject.IsSelected = SelectionRect.IntersectsWith(r);
                }
                else if (uiObject is CurveElement curve)
                {
                    uiObject.IsSelected = curve.Points.Any(p =>
                    {
                        var globalPoint = new Point(
                            p.X + curve.X,
                            p.Y + curve.Y
                        );

                        return SelectionRect.Contains(globalPoint);
                    });
                }
            }
        }
        private bool LineIntersectsRect(Point p1, Point p2, Rect rect)
        {
            if (rect.Contains(p1) || rect.Contains(p2))
                return true;

            return LineIntersectsLine(p1, p2, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top)) ||
                   LineIntersectsLine(p1, p2, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom)) ||
                   LineIntersectsLine(p1, p2, new Point(rect.Right, rect.Bottom), new Point(rect.Left, rect.Bottom)) ||
                   LineIntersectsLine(p1, p2, new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top));
        }

        private bool LineIntersectsLine(Point a1, Point a2, Point b1, Point b2)
        {
            double d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);
            if (d == 0) return false;

            double u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
            double v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;

            return (u >= 0 && u <= 1 && v >= 0 && v <= 1);
        }

        private bool LineNearRect(Point p1, Point p2, Rect rect, double tolerance = 5)
        {
            Rect expanded = new Rect(
                rect.X - tolerance,
                rect.Y - tolerance,
                rect.Width + tolerance * 2,
                rect.Height + tolerance * 2);

            return LineIntersectsRect(p1, p2, expanded);
        }
        public void StopDrawSelection()
        {
            SelectionRect = new Rect();
            IsDrawingSelection = false;
        }
        public void StopSelection()
        {
            IsSelecting = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
