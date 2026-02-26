using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.ViewModels;
using NetOptimizer.Views.DopViews;
using System.IO;
using System.Text;

namespace NetOptimizer.Services
{
    public class YamlNetworkManager : IYamlManager, IDisposable
    {
        private FileStream _activeStream;
        private string _currentLockedPath;
        private Dictionary<KeywordsStructureType, List<string>> dictionaryKeywords = new Dictionary<KeywordsStructureType, List<string>>();
        public NetworkMap ResultMap { get; private set; } = new NetworkMap { Devices = new List<Device>() };


        private readonly IWindowNavigator _windowNavigator;
        public YamlNetworkManager(IWindowNavigator windowNavigator)
        {
            _windowNavigator = windowNavigator;
            InitializeDictionaryKeywords();
        }
        private void InitializeDictionaryKeywords()
        {
            dictionaryKeywords.Add(KeywordsStructureType.NetworkSection, new List<string>() { "name" });
            dictionaryKeywords.Add(KeywordsStructureType.NodesSection, new List<string>() { "id", "type", "model", "ip", "port_type", "port_count" });
            dictionaryKeywords.Add(KeywordsStructureType.LinksSection, new List<string>() { "src", "src_port", "dst", "dst_port" });
        }
        public async Task<NetworkMap> CheckConfigAsync(string configText)
        {
            ResultMap = new NetworkMap { Devices = new List<Device>() };
            var rawLinks = new List<RawLink>();

            using (StringReader configReader = new StringReader(configText))
            {
                string firstLine = await configReader.ReadLineAsync();
                if (firstLine?.Trim() != "netoptimizer") { return null; }

                string line;
                string currentSection = "";
                var buffer = new Dictionary<string, string>();

                while ((line = await configReader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    int indent = line.TakeWhile(char.IsWhiteSpace).Count();
                    string trimmed = line.Trim();

                    if (indent == 0 && trimmed.EndsWith(":"))
                    {
                        var resultcheck = SaveBufferToModel(currentSection, buffer, rawLinks);
                        if (resultcheck == false)
                        {
                            return null;
                        }
                        currentSection = trimmed.TrimEnd(':');
                        continue;
                    }

                    if (trimmed.StartsWith("-"))
                    {
                        var resultcheck = SaveBufferToModel(currentSection, buffer, rawLinks);
                        if (resultcheck == false)
                        {
                            return null;
                        }
                        trimmed = trimmed.TrimStart('-', ' ').Trim();
                    }

                    if (trimmed.Contains(":"))
                    {
                        var parts = trimmed.Split(':', 2);
                        string key = parts[0].Trim();
                        string value = parts[1].Trim().Trim('"');
                        buffer[key] = value;
                    }
                }
                SaveBufferToModel(currentSection, buffer, rawLinks);
            }

            FinalizeConnections(rawLinks);
            return ResultMap;
        }

        private bool SaveBufferToModel(string section, Dictionary<string, string> buffer, List<RawLink> rawLinks)
        {
            if (buffer.Count == 0) return true;
            var sectionType = section switch
            {
                "network" => KeywordsStructureType.NetworkSection,
                "nodes" => KeywordsStructureType.NodesSection,
                "links" => KeywordsStructureType.LinksSection,
                _ => (KeywordsStructureType?)null
            };

            if (sectionType.HasValue)
            {
                foreach (var key in buffer.Keys)
                {
                    if (!dictionaryKeywords[sectionType.Value].Contains(key))
                    {
                        _windowNavigator.ShowModalView<ErrorWindow, ErrorWindowViewModel>(
                            $"Неизвестное свойство: {key} \n В блоке: {section}");
                        return false;
                    }
                }
            }

            if (section == "nodes")
            {
                string id = buffer.GetValueOrDefault("id", "Unknown");
                string typeStr = buffer.GetValueOrDefault("type", "pc").ToLower();
                string devicemodel = buffer.GetValueOrDefault("model", "UnknowModel").ToLower();
                int ports = int.TryParse(buffer.GetValueOrDefault("port_count", "1"), out var p) ? p : 1;

                Device device = typeStr switch
                {
                    "router" => new RouterDevice(id, ports),
                    "switch" => new SwitchDevice(id, ports),
                    //"server" => new ServerDevice(id, ports),
                    "pc" => new PcDevice(id)
                };
                device.DeviceModel = devicemodel;
                ResultMap.Devices.Add(device);
            }
            else if (section == "links")
            {
                rawLinks.Add(new RawLink
                {
                    Source = buffer.GetValueOrDefault("src"),
                    Target = buffer.GetValueOrDefault("dst"),
                    SourcePort = buffer.GetValueOrDefault("src_port"),
                    TargetPort = buffer.GetValueOrDefault("dst_port")
                });
            }
            else if (section == "network")
            {
                if (buffer.ContainsKey("name")) ResultMap.NetworkName = buffer["name"];
            }

            buffer.Clear();
            return true;
        }
        private void FinalizeConnections(List<RawLink> rawLinks)
        {
            foreach (var link in rawLinks)
            {
                var deviceA = ResultMap.Devices.Find(d => d.Name == link.Source);
                var deviceB = ResultMap.Devices.Find(d => d.Name == link.Target);

                if (deviceA == null || deviceB == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка: Устройство {link.Source} или {link.Target} не найдено.");
                    continue;
                }
                string portNumA = link.SourcePort;
                string portNumB = link.TargetPort;

                var portA = deviceA.Ports.Find(p => p.PortNumber == portNumA);
                var portB = deviceB.Ports.Find(p => p.PortNumber == portNumB);
                if (portA != null && portB != null)
                {
                    try
                    {
                        portA.LinkTo(portB);
                        System.Diagnostics.Debug.WriteLine($"Связано: {deviceA.Name}({portNumA}) <-> {deviceB.Name}({portNumB})");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка связи: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Порт не найден: {portNumA} на {deviceA.Name} или {portNumB} на {deviceB.Name}");
                }
            }
        }



        public async Task<string> ReadYamlFileAsync(string pathToFile)
        {
            try
            {
                if (_activeStream != null && _currentLockedPath == pathToFile)
                {
                    _activeStream.Position = 0;
                    using (var reader = new StreamReader(_activeStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
                using (var stream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка чтения: {ex.Message}");
                return "произошла ошибка";
            }
        }


        public async Task<bool> CreateYamlFile(string path, string fileName, string yamltext)
        {
            try
            {
                string fullPath = Path.Combine(path, fileName);
                if (_activeStream == null || _currentLockedPath != fullPath)
                {
                    _activeStream?.Dispose();
                    _activeStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    _currentLockedPath = fullPath;
                }
                _activeStream.SetLength(0);
                using (var writer = new StreamWriter(_activeStream, Encoding.UTF8, 1024, leaveOpen: true))
                {
                    await writer.WriteAsync(yamltext);
                    await writer.FlushAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }
        public void Dispose()
        {
            _activeStream?.Dispose();
        }


    }
}
