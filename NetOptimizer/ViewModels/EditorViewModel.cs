using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AvailableDevicesForEditorDto> AvailableDevices { get; } = new ObservableCollection<AvailableDevicesForEditorDto>();
        public ObservableCollection<AvailableTypesOfObjectForEditorDto> AvailableTypes { get; } = new ObservableCollection<AvailableTypesOfObjectForEditorDto>();

        public ICommand CreateNetworkByUserPropertiesCommand { get; }
        public EditorViewModel()
        {
            CreateNetworkByUserPropertiesCommand = new AsyncRelayCommand(CreateNetworkByUserProperties);
        }
        private async Task CreateNetworkByUserProperties()
        {

        }
        public void InitializeAvailableTypes()
        {
            var types = new List<AvailableTypesOfObjectForEditorDto>
            {
                new() { TypeOfObject = "Квартира", Type = PlacementType.Apartment },
                new() { TypeOfObject = "Малый офис", Type = PlacementType.SmallOffice },
                new() { TypeOfObject = "Средний офис", Type = PlacementType.MediumOffice },
                new() { TypeOfObject = "Предприятие", Type = PlacementType.Enterprise }
            };

            foreach (var t in types)
            {
                t.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(AvailableTypesOfObjectForEditorDto.IsSelected))
                    {
                        var changedItem = (AvailableTypesOfObjectForEditorDto)s;
                        if (changedItem.IsSelected)
                            OnTypeSelected(changedItem);
                    }
                };
                AvailableTypes.Add(t);
            }
        }

        private void OnTypeSelected(AvailableTypesOfObjectForEditorDto selected)
        {
            foreach (var t in AvailableTypes)
            {
                if (t != selected) t.IsSelected = false;
            }
        }
        public void InitializeAllAvailableDevices()
        {
            var limits = new Dictionary<DeviceType, int>  //Девайс - максимальное кол-во
            {
                { DeviceType.PC, 500 },
                { DeviceType.IpVideoCam, 100 },
                { DeviceType.Router, 30 },
                { DeviceType.Switch, 30 },
                { DeviceType.Printer, 30 },
                { DeviceType.AccessPoint, 30 },
                { DeviceType.Server, 30 }
            };

            foreach (var entry in limits)
            {
                AvailableDevices.Add(new AvailableDevicesForEditorDto()
                {
                    DeviceName = entry.Key.ToString(),
                    Type = entry.Key,
                    MaxCount = entry.Value
                });
            }
        }       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
