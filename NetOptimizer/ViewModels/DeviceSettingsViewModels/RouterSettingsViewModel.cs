using NetOptimizer.Enums;
using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace NetOptimizer.ViewModels.DeviceSettingss
{
    public class RouterSettingsViewModel : DeviceSettingsBase, INotifyPropertyChanged
    {
        private readonly List<RouterResponceDto> _catalog;
        private string _selectedVendor;
        private string _name;
        private RouterResponceDto _selectedModel;
        public List<string> AvailableVendors { get; }
        public List<RouterResponceDto> FilteredModels => _catalog.Where(x => x.Vendor == SelectedVendor).ToList();
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
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public RouterResponceDto SelectedModelFromCatalog
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Vendor));
                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(IsManaged));
                OnPropertyChanged(nameof(TotalPorts));
                OnPropertyChanged(nameof(Ports));
                OnPropertyChanged(nameof(WifiOptions));
                OnPropertyChanged(nameof(Performance));
                OnPropertyChanged(nameof(ProtocolSupport));
                OnPropertyChanged(nameof(AveragePrice));
            }
        }
        public string Vendor => SelectedModelFromCatalog?.Vendor;
        public string Model => SelectedModelFromCatalog?.Model;
        public bool IsManaged => (bool)(SelectedModelFromCatalog?.IsManaged);
        public int TotalPorts => (SelectedModelFromCatalog?.TotalPorts) ?? 0;
        public List<PortDto> Ports => SelectedModelFromCatalog?.Ports;
        public WifiOptions WifiOptions => SelectedModelFromCatalog?.WifiOptions;
        public PerformanceSpecs Performance => SelectedModelFromCatalog?.Performance;
        public ProtocolSupport ProtocolSupport => SelectedModelFromCatalog?.ProtocolSupport;
        public decimal AveragePrice => (SelectedModelFromCatalog?.AveragePrice) ?? 0; 
        public RouterSettingsViewModel(List<RouterResponceDto> catalog)
        {
            _catalog = catalog ?? new List<RouterResponceDto>();
            AvailableVendors = _catalog.Select(x => x.Vendor).Distinct().ToList();
        }

        public RouterSettings GetSettings()
        {
            if (SelectedModelFromCatalog == null) return null;
            return new RouterSettings
            {
                Model = SelectedModelFromCatalog?.Model ?? "Custom",
                Vendor = SelectedVendor,
                TotalPorts = this.TotalPorts,
                AveragePrice = this.AveragePrice,
                Performance = this.Performance,
                ProtocolSupport = this.ProtocolSupport,
                IsManaged = this.IsManaged,
                WifiOptions = this.WifiOptions,               
                Ports = SelectedModelFromCatalog?.Ports ?? new List<PortDto>()
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
