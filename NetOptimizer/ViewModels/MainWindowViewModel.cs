using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _projectName;
        private string _currentYamlText;
        private string _currentFilePath;
        public string ProjectName { get => _projectName; set { _projectName = value; OnPropertyChanged(); } }
        public string CurrentYamlText { get => _currentYamlText; set { _currentYamlText = value; OnPropertyChanged(); _ = AutoSaveAsync(); } }
        public string CurrentFilePath { get => _currentFilePath; set { _currentFilePath = value; OnPropertyChanged(); } }

        public ObservableCollection<DeviceOnCanvas> DevicesOnCanvas { get; } = new ObservableCollection<DeviceOnCanvas>();

        public event EventHandler<DeviceOnCanvas> DeviceAdded;
        public event Action<DeviceOnCanvas, DeviceOnCanvas> ConnectionRequested;

        public EditorViewModel editorViewModel { get; }
        public SandboxViewModel sandboxViewModel { get; }
        public ICommand ApplyAndBuildYamlFileCommand { get; }
        public ICommand OpenFinderFileDialogCommand { get; }

        private readonly IWindowNavigator _windowNavigator;
        private readonly IYamlManager _yamlManager;
        private readonly IFileService _fileService;
        public MainWindowViewModel(IWindowNavigator windowNavigator, IYamlManager yamlManager, IFileService fileservice)
        {
            _windowNavigator = windowNavigator;
            _yamlManager = yamlManager;
            _fileService = fileservice;
            editorViewModel = new EditorViewModel();
            sandboxViewModel = new SandboxViewModel(_windowNavigator);
            EventAggregator.Instance.DeviceCreated += OnDeviceCreated;

            ApplyAndBuildYamlFileCommand = new AsyncRelayCommand(ApplyAndBuildNetwork);
            OpenFinderFileDialogCommand = new RelayCommand(OpenFinderFileFileDialog);
            InitializeNewProject();

            sandboxViewModel.InitializeAllAvailableDevices();
            editorViewModel.InitializeAvailableTypes();
            editorViewModel.InitializeAllAvailableDevices();
        }
        public void UpdateYamlFromCanvas()
        {
            StringBuilder yaml = new StringBuilder();
            yaml.AppendLine("netoptimizer");
            yaml.AppendLine("");
            yaml.AppendLine("network:");
            yaml.AppendLine($"  name: \"{ProjectName}\"");
            yaml.AppendLine();

            // Секция Nodes
            yaml.AppendLine("nodes:");
            foreach (var device in DevicesOnCanvas)
            {
                yaml.AppendLine($"  - id: \"{device.LogicDevice.Name}\"");
                yaml.AppendLine($"    type: \"{device.LogicDevice.GetType().Name.Replace("Device", "").ToLower()}\"");
                yaml.AppendLine($"    model: \"{device.LogicDevice.DeviceModel}\"");
                yaml.AppendLine($"    port_count: {device.LogicDevice.Ports.Count}");
                yaml.AppendLine($"");
            }
            yaml.AppendLine();

            // Секция Links
            yaml.AppendLine("links:");
            var processedLinks = new HashSet<string>();

            foreach (var device in DevicesOnCanvas)
            {
                foreach (var port in device.LogicDevice.Ports)
                {
                    if (port.IsLinked && port.IsInitiator)
                    {
                        // Создаем уникальный ключ для пары портов, чтобы не дублировать связь в YAML
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
            string fileName = "NewGnoProject.yml";
            ProjectName = fileName;
            CurrentFilePath = System.IO.Path.Combine(docPath, fileName);
            CurrentYamlText = "network:\n  nodes: []\n  connections: []";
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
            string path = _fileService.OpenFileDialog("YAML files (*.yml)|*.yml");
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
        private void OnDeviceCreated(DeviceToAddDto dto, DeviceSettingsBase settings)
        {
            Device logicDevice = dto.Type switch
            {
                DeviceType.PC => new PcDevice(((PcSettingModel)settings).Name, configur => configur.DeviceModel = "SomeModel"),
                _ => null
            };

            if (logicDevice == null) return;

            var deviceOnCanvas = new DeviceOnCanvas(logicDevice, x: 500, y: 500);
            DevicesOnCanvas.Add(deviceOnCanvas);
            DeviceAdded?.Invoke(this, deviceOnCanvas);
            UpdateYamlFromCanvas();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

