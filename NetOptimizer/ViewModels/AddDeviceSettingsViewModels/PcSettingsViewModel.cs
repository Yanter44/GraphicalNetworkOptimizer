using NetOptimizer.Models.DeviceModels.DeviceSettings;
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.ViewModels.DeviceSettingss
{
    public class PcSettingsViewModel :  DeviceSettingsBase, INotifyPropertyChanged
    {
        private readonly List<PcResponceDto> _catalog;
        public List<string> AvailableVendors { get; }
        private PcResponceDto _selectedModel;
        private string _selectedVendor;
        private string _name;
        private List<PcResponceDto> _filteredModels;
        public List<PcResponceDto> FilteredModels
        {
            get => _filteredModels;
            private set { _filteredModels = value; OnPropertyChanged(); }
        }
        public PcResponceDto SelectedModelFromCatalog
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Vendor));
                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(Ports));
                OnPropertyChanged(nameof(HardwareSpecs));
                OnPropertyChanged(nameof(WifiOptions));
                OnPropertyChanged(nameof(AveragePrice));
            }
        }
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public string SelectedVendor
        {
            get => _selectedVendor;
            set
            {
                _selectedVendor = value;
                OnPropertyChanged();
                FilteredModels = _catalog.Where(x => x.Vendor == value).ToList();
                SelectedModelFromCatalog = null;
            }
        }
        public string? Vendor => SelectedModelFromCatalog?.Vendor;
        public string? Model => SelectedModelFromCatalog?.Model;
        public List<PortDto>? Ports => SelectedModelFromCatalog?.Ports;
        public PcHardwareSpecs? HardwareSpecs => SelectedModelFromCatalog?.HardwareSpecs;
        public PcWifiOptions? WifiOptions => SelectedModelFromCatalog?.WifiOptions;
        public decimal? AveragePrice => SelectedModelFromCatalog?.AveragePrice;
        public PcSettingsViewModel(List<PcResponceDto> catalog)
        {
            _catalog = catalog ?? new List<PcResponceDto>();
            AvailableVendors = _catalog.Select(x => x.Vendor).Distinct().ToList();
        }
        public PcSettings GetSettings()
        {
            if (SelectedModelFromCatalog == null) return null;
            return new PcSettings
            {
                Vendor = SelectedVendor,
                Model = SelectedModelFromCatalog.Model,
                Ports = this.Ports,
                WifiOptions = this.WifiOptions,
                HardwareSpecs = this.HardwareSpecs,
                AveragePrice = (decimal)this.AveragePrice,
                
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
