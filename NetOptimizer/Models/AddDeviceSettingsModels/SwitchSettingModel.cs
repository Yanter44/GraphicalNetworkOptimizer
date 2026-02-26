using NetOptimizer.Enums;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetOptimizer.Models.AddDeviceSettingsModels
{
    public class SwitchSettingModel : DeviceSettingsBase, INotifyPropertyChanged
    {
        private string _name;
        private string _selectedVendor;
        private int _totalPorts;
        private int _sfpPortsCount;
        private decimal _averagePrice;
        public string Model { get; set; }
        public DeviceLayer DeviceLayer { get; set; }

        private CommutatorResponceDto _selectedModel;
        public string SelectedVendor { get => _selectedVendor; set { _selectedVendor = value; OnPropertyChanged(); OnPropertyChanged(nameof(FilteredModels)); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        public int TotalPorts { get => _totalPorts; set { _totalPorts = value; OnPropertyChanged(); } }

        public int SfpPortsCount { get => _sfpPortsCount; set { _sfpPortsCount = value; OnPropertyChanged(); } }

        public bool SupportsPoe { get; set; }
        public decimal AveragePrice { get => _averagePrice; set { _averagePrice = value; OnPropertyChanged(); } }
        public CommutatorResponceDto SelectedModelFromCatalog { get => _selectedModel; set { _selectedModel = value; OnPropertyChanged(); if (value != null) FillFieldsFromDto(value); } }

        public List<string> AvailableVendors { get; }
        private readonly List<CommutatorResponceDto> _catalog;
        public List<CommutatorResponceDto> FilteredModels =>_catalog.Where(x => x.Vendor == SelectedVendor).ToList();

        public SwitchSettingModel(List<CommutatorResponceDto> catalog)
        {
            _catalog = catalog ?? new List<CommutatorResponceDto>();
            AvailableVendors = _catalog.Select(x => x.Vendor).Distinct().ToList();
        }

        private void FillFieldsFromDto(CommutatorResponceDto dto)
        {
            TotalPorts = dto.TotalPorts;
            SfpPortsCount = dto.SfpPorts;
            SupportsPoe = dto.SupportsPoe;
            AveragePrice = dto.AveragePrice;
        }
  

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
