using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Models.Enums;
using NetOptimizer.Models.UIElements;
using NetOptimizer.Services;
using NetOptimizer.ViewModels.DeviceSettingss;
using NetOptimizer.ViewModels.MainWindoww;
using NetOptimizer.Views.DopViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace NetOptimizer.ViewModels.MainWindow
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _projectName;
        private string _currentYamlText;
        private string _currentFilePath;
        public string ProjectName { get => _projectName; set { _projectName = value; OnPropertyChanged(); } }
        public string CurrentYamlText { get => _currentYamlText; set { _currentYamlText = value; OnPropertyChanged(); _ = AutoSaveAsync(); } }
        public string CurrentFilePath { get => _currentFilePath; set { _currentFilePath = value; OnPropertyChanged(); } }

        private CanvasSettings _canvasSettings;
        public CanvasSettings CanvasSettings { get => _canvasSettings; set { _canvasSettings = value; OnPropertyChanged(); } } 
        public ObservableCollection<DeviceConnection> Connections { get; } = new ObservableCollection<DeviceConnection>();
        public ObservableCollection<DeviceOnCanvas> DevicesOnCanvas => CanvasVM.Devices;
         
        public event Action<DeviceOnCanvas[]> DevicesUpdated;

        public event EventHandler<DeviceOnCanvas> DeviceAdded;

        public event Action<DeviceOnCanvas, DeviceOnCanvas> ConnectionRequested;
        public EditorViewModel editorViewModel { get; }
        public SandboxViewModel sandboxViewModel { get; }
        public CanvasViewModel CanvasVM { get; }
        public ICommand ApplyAndBuildYamlFileCommand { get; }
        public ICommand OpenFinderFileDialogCommand { get; }
        
        private readonly IWindowNavigator _windowNavigator;
        private readonly IYamlManager _yamlManager;
        private readonly IFileService _fileService;
        private readonly DeviceCatalogService _deviceCatalogService;
        public MainWindowViewModel(IWindowNavigator windowNavigator, IYamlManager yamlManager,
                                   IFileService fileservice, EditorViewModel editorVM,
                                   SandboxViewModel sandboxVM, CanvasViewModel canvasVM, DeviceCatalogService deviceCatalogService)
        {
            _deviceCatalogService = deviceCatalogService;
            _windowNavigator = windowNavigator;
            _yamlManager = yamlManager;
            _fileService = fileservice;

            CanvasVM = canvasVM;
            CanvasVM.CanvasWidth = 1000;
            CanvasVM.CanvasHeight = 1000;

            editorViewModel = editorVM;
            sandboxViewModel = sandboxVM;
            EventAggregator.Instance.DeviceCreated += (dto, settings) => OnDeviceCreated(dto, settings);

            ApplyAndBuildYamlFileCommand = new AsyncRelayCommand(ApplyAndBuildNetwork);
            OpenFinderFileDialogCommand = new RelayCommand(OpenFinderFileFileDialog);
            InitializeNewProject();

            sandboxViewModel.InitializeAllAvailableDevices();
            sandboxViewModel.InitializeAllAvailableTools();
            sandboxViewModel.StartDrawTool += OnStartDraw;

            editorViewModel.InitializeAvailableTypes();
            editorViewModel.InitializeAllAvailableDevices();
            editorViewModel.GenerationRequested += OnAutoGenerationRequested;

            UpdateYamlFromCanvas();
        }
        private void OnStartDraw(UIToolElementToAddDto tool)
        {
            CanvasVM.SetCurrentTool(tool);
        }
        private Task OnAutoGenerationRequested(List<AvailableDevicesForEditorDto> EndlesnodeDevices, decimal budget)
        {
            var pcs = EndlesnodeDevices.Where(d => d.Type == DeviceType.PC).ToList();
            var printers = EndlesnodeDevices.Where(d => d.Type == DeviceType.Printer).ToList();

            TopologyType networkTopology;
            switch (pcs.Count())
            {
                case <= 24:
                    networkTopology = TopologyType.SingleStar;
                    break;
                case > 28 and < 144:
                    networkTopology = TopologyType.HierarchicalTree;
                    break;
            }
            var theCheapestPc = _deviceCatalogService.AvailablePcs.OrderBy(x => x.AveragePrice).FirstOrDefault();
            var theCheapestPrinter = _deviceCatalogService.AvailablePrinters.OrderBy(x => x.AveragePrice).FirstOrDefault();
            int totalRequiredPorts = 0;
            if (theCheapestPc != null)
            {
                var totalPcsPortsCount = theCheapestPc.Ports.Sum(port => port.Count);
                totalRequiredPorts += pcs.Sum(pc => pc.Count) * theCheapestPc.Ports.Count;
            }

            if (theCheapestPrinter != null)
            {
                var totalPrinterPortsCount = theCheapestPrinter.Ports.Sum(port => port.Count);
                totalRequiredPorts += printers.Count * totalPrinterPortsCount;
            }
            int portsWithReserve = (int)Math.Ceiling(totalRequiredPorts * 1.2);

            decimal totalNodesCost = (pcs.Sum(pc => pc.Count) * (decimal)theCheapestPc.AveragePrice) + 
                                     (printers.Sum(p => p.Count) * (decimal)theCheapestPrinter.AveragePrice);

            decimal budgetForNetwork = budget - totalNodesCost;

            return Task.CompletedTask;
        }
        public void UpdateYamlFromCanvas()
        {
            StringBuilder yaml = new StringBuilder();
            yaml.AppendLine("netoptimizer");
            yaml.AppendLine();
            yaml.AppendLine("network:");
            yaml.AppendLine($"  name: \"{ProjectName}\"");
            yaml.AppendLine();
            yaml.AppendLine("canvas:");
            yaml.AppendLine($"  width: {_canvasSettings.Width}");
            yaml.AppendLine($"  height: {_canvasSettings.Height}");
            yaml.AppendLine();

            yaml.AppendLine("nodes:");
            foreach (var device in DevicesOnCanvas)
            {
                var logic = device.LogicDevice;

                yaml.AppendLine($"  - id: \"{logic.Name}\"");
                yaml.AppendLine($"    type: \"{logic.GetType().Name.Replace("Device", "").ToLower()}\"");
                yaml.AppendLine($"    model: \"{logic.DeviceModel}\"");
                yaml.AppendLine($"    vendor: \"{logic.Vendor}\""); 

                if (logic is SwitchDevice sw)
                {
                    int sfpCount = sw.Ports.Count(p => p.Type == PortType.SFPPlus);
                    int rj45Count = sw.Ports.Count(p => p.Type == PortType.RJ45);

                    yaml.AppendLine($"    port_count: {rj45Count + sfpCount}");
                    yaml.AppendLine($"    sfp_count: {sfpCount}");
                    yaml.AppendLine($"    poe: {sw.SupportsPoe.ToString().ToLower()}");
                    yaml.AppendLine($"    layer: \"{sw.Layer}\"");
                }
                yaml.AppendLine($"");
            }
            yaml.AppendLine("links:");
            var processedLinks = new HashSet<string>();

            foreach (var device in DevicesOnCanvas)
            {
                foreach (var port in device.LogicDevice.Ports)
                {
                    if (port.IsLinked && port.IsInitiator)
                    {
                        var linkKey = string.Join("-", new[] { port.GetHashCode(), port.ConnectedTo.GetHashCode() }.OrderBy(x => x));

                        if (!processedLinks.Contains(linkKey))
                        {
                            yaml.AppendLine($"  - src: \"{device.LogicDevice.Name}\"");
                            yaml.AppendLine($"    src_port: \"{port.PortNumber}\"");
                            yaml.AppendLine($"    dst: \"{port.ConnectedTo.Owner.Name}\"");
                            yaml.AppendLine($"    dst_port: \"{port.ConnectedTo.PortNumber}\"");
                            yaml.AppendLine($"");

                            processedLinks.Add(linkKey);
                        }
                    }
                }
            }
            CurrentYamlText = yaml.ToString();
        }
        private async void InitializeNewProject()
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = "NewGnoProject.ymlnet";
            ProjectName = fileName;
            CurrentFilePath = System.IO.Path.Combine(docPath, fileName);
            CanvasSettings = new CanvasSettings(1000, 1000);
            CurrentYamlText = "netoptimizer\n canvas:\n network:\n  nodes: \n  links: \n ";
            await _yamlManager.CreateYamlFile(docPath, fileName, CurrentYamlText);
        }
        private async Task ApplyAndBuildNetwork()
        {
            var resultRead = await _yamlManager.ReadYamlFileAsync(CurrentFilePath);
            var resultWrite = await _yamlManager.CheckConfigAsync(resultRead);
            if (resultWrite != null)
            {
                BuildNetwork(resultWrite);
            }
        }
        private async void OpenFinderFileFileDialog()
        {
            string path = _fileService.OpenFileDialog("ymlnet files (*.ymlnet)|*.ymlnet");
            var resultRead = await _yamlManager.ReadYamlFileAsync(path);
            CurrentYamlText = resultRead;
            CurrentFilePath = path;
        }
        private async Task AutoSaveAsync()
        {
            if (string.IsNullOrEmpty(CurrentFilePath)) return;

            string directory = System.IO.Path.GetDirectoryName(CurrentFilePath);
            string fileName = System.IO.Path.GetFileName(CurrentFilePath);

            await _yamlManager.CreateYamlFile(directory, fileName, _currentYamlText);
        }
        public void BuildNetwork(NetworkMap networkmap)
        {
            CanvasSettings.Width = networkmap.CanvasSettings.Width;
            CanvasSettings.Height = networkmap.CanvasSettings.Height;
            var visualLookup = new Dictionary<string, DeviceOnCanvas>();
            int startX = 100;

            foreach (var item in networkmap.Devices)
            {
                var visual = new DeviceOnCanvas(item, startX, 200);
                visualLookup[item.Name] = visual;

                DevicesOnCanvas.Add(visual);
                DeviceAdded?.Invoke(this, visual);

                startX += 200;
            }
            var processedLinks = new HashSet<int>();
            foreach (var visual in DevicesOnCanvas)
            {
                foreach (var port in visual.LogicDevice.Ports)
                {
                    if (port.ConnectedTo == null) continue;

                    int linkId = port.GetHashCode() ^ port.ConnectedTo.GetHashCode();

                    if (!processedLinks.Contains(linkId))
                    {
                        var sourceVisual = visual;
                        var targetVisual = DevicesOnCanvas.FirstOrDefault(d => d.LogicDevice == port.ConnectedTo.Owner);

                        if (targetVisual != null)
                        {

                            ConnectionRequested?.Invoke(sourceVisual, targetVisual);
                            processedLinks.Add(linkId);
                        }
                    }
                }
            }
        }
        public void DrawAllConnectionsForDevice(DeviceOnCanvas deviceOnCanvas)
        {
            foreach (var port in deviceOnCanvas.LogicDevice.Ports)
            {
                if (port.IsLinked)
                {
                    var remotePort = port.ConnectedTo;
                    var targetVisual = DevicesOnCanvas.FirstOrDefault(d => d.LogicDevice == remotePort.Owner);

                    if (targetVisual != null)
                    {
                        ConnectionRequested?.Invoke(deviceOnCanvas, targetVisual);
                    }
                }
            }
        }

        public bool TryConnectPorts(DeviceOnCanvas sourceDevice, Port sourcePort, DeviceOnCanvas targetDevice, Port targetPort, PointCollection intermediatePoints = null)
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
                    TargetPort = targetPort,
                };
                intermediatePoints?.Skip(1).Take(intermediatePoints.Count - 2)
                                  .ToList()
                                  .ForEach(p => conn.AddIntermediatePoint(p));

                conn.UpdatePoints();
                Connections.Add(conn);
                UpdateYamlFromCanvas();
                return true;
            }
            catch (Exception ex)
            {
                return ShowError(ex.Message);
            }

            bool ShowError(string message)  // <-- локальная функция
            {
                _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>(message);
                return false;
            }
        }
        
        public void ShowPortError(Port port) =>  _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>($"Ошибка: Порт {port.PortName} уже занят!");
        public void RemoveDevice(DeviceOnCanvas device)
        {
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

            DevicesOnCanvas.Remove(device);
            UpdateYamlFromCanvas();
            DevicesUpdated?.Invoke(neighbors);
        }

        public void MoveDevice(DeviceOnCanvas device, double dx, double dy)
        {
            var target = DevicesOnCanvas.FirstOrDefault(d => d == device);
            if (target == null) return;

            target.X += dx;
            target.Y += dy;
        }
        public void SelectDevice(DeviceOnCanvas device, bool isSelected)
        {
            if (device != null)
                device.IsSelected = isSelected;
        }
        private void OnDeviceCreated(DeviceToAddDto dto, DeviceSettingsBase settings, double x = 500, double y = 500)
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
            DevicesOnCanvas.Add(deviceOnCanvas);
            UpdateYamlFromCanvas();
        }
     
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

