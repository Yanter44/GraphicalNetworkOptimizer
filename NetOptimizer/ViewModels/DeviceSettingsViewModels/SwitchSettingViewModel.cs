using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.Dtos;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetOptimizer.ViewModels.DeviceSettingss
{
    public class SwitchSettingViewModel : DeviceSettingsBase, INotifyPropertyChanged
    {
        private readonly List<CommutatorResponceDto> _catalog;
        private string _name;
        private string _selectedVendor;
        private CommutatorResponceDto _selectedModel;
        public List<string> AvailableVendors { get; }

        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string SelectedVendor
        {
            get => _selectedVendor;
            set
            {
                _selectedVendor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredModels));
                _selectedModel = null; 
            }
        }
        public bool SupportsPoe => SelectedModelFromCatalog?.PoeSpecs.SupportsPoe ?? false;
        public int PoeBudgetW => SelectedModelFromCatalog?.PoeSpecs.PoeBudgetW ?? 0;
        public decimal ThroughputGbps => SelectedModelFromCatalog?.PerformanceSpecs.ThroughputGbps ?? 0;
        public int MacTableSize => SelectedModelFromCatalog?.PerformanceSpecs.MacTableSize ?? 0;
        public int MaxVlans => SelectedModelFromCatalog?.PerformanceSpecs.MaxVlans ?? 0;
        public bool IsManaged => SelectedModelFromCatalog?.IsManaged ?? false;
        public List<CommutatorResponceDto> FilteredModels => _catalog.Where(x => x.Vendor == SelectedVendor).ToList();


        public CommutatorResponceDto SelectedModelFromCatalog
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPorts));
                OnPropertyChanged(nameof(SupportsPoe));
                OnPropertyChanged(nameof(Ports));
                OnPropertyChanged(nameof(AveragePrice));
                OnPropertyChanged(nameof(DeviceLayer));
                OnPropertyChanged(nameof(PoeBudgetW));
                OnPropertyChanged(nameof(ThroughputGbps));
                OnPropertyChanged(nameof(MacTableSize));
                OnPropertyChanged(nameof(MaxVlans));
                OnPropertyChanged(nameof(IsManaged));
            }
        }
        public int TotalPorts => SelectedModelFromCatalog?.Ports.Sum(x => x.Count) ?? 0;
        public List<PortDto> Ports => SelectedModelFromCatalog?.Ports;
        public decimal AveragePrice => SelectedModelFromCatalog?.AveragePrice ?? 0;
        public DeviceLayer DeviceLayer => SelectedModelFromCatalog?.Layer switch
        {
            3 => DeviceLayer.L3,
            _ => DeviceLayer.L2 
        };
        public SwitchSettingViewModel(List<CommutatorResponceDto> catalog)
        {
            _catalog = catalog ?? new List<CommutatorResponceDto>();
            AvailableVendors = _catalog.Select(x => x.Vendor).Distinct().ToList();
        }

        public SwitchSettings GetSettings()
        {
            if (SelectedModelFromCatalog == null) return null;
            return new SwitchSettings
            {
                Model = SelectedModelFromCatalog?.Model ?? "Custom",
                Vendor = SelectedVendor,
                PoeSpecs = SelectedModelFromCatalog.PoeSpecs,
                PerformanceSpecs = SelectedModelFromCatalog.PerformanceSpecs,
                ProtocolSupport = SelectedModelFromCatalog.ProtocolSupport,
                SwitchRoleType = SelectedModelFromCatalog.SwitchRoleType,
                AveragePrice = SelectedModelFromCatalog.AveragePrice,
                DeviceLayer = SelectedModelFromCatalog.Layer switch { 3 => DeviceLayer.L3, _ => DeviceLayer.L2 },
                Ports = SelectedModelFromCatalog?.Ports ?? new List<PortDto>()
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
