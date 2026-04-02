using NetOptimizer.Common;
using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.Dtos;
using NetOptimizer.Services;
using NetOptimizer.ViewModels.AddNewGroupWindoww;
using NetOptimizer.ViewModels.ConnectToGroupWindoww;
using NetOptimizer.ViewModels.DeviceSettingss;
using NetOptimizer.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.MainWindow
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        private readonly DeviceCatalogService _catalogService;
        private readonly IWindowNavigator _windowNavigator;

        private decimal _Budget;
        public decimal Budget { get => _Budget; set { _Budget = value; OnPropertyChanged(); } }
        public ObservableCollection<AvailableDevicesForEditorDto> AvailableDevices { get; } = new ObservableCollection<AvailableDevicesForEditorDto>();
        public ObservableCollection<AvailableTypesOfObjectForEditorDto> AvailableTypes { get; } = new ObservableCollection<AvailableTypesOfObjectForEditorDto>();
        public ObservableCollection<DeviceGroup> DeviceGroups { get; } = new ObservableCollection<DeviceGroup>();

        public event Func<List<AvailableDevicesForEditorDto>, decimal, Task> GenerationRequested;
        public ICommand CreateNetworkByUserPropertiesCommand { get; }
        public ICommand OpenAddGroupWindowCommand { get; }
        public ICommand RemoveDeviceGroupCommand { get; }
        public ICommand OpenConnectGroupWindowCommand { get; }
        public ICommand DisconnectFromGroupCommand { get; }
        public EditorViewModel(IWindowNavigator windowNavigator, DeviceCatalogService catalogService)
        {
            _windowNavigator = windowNavigator; 
            _catalogService = catalogService;          
            CreateNetworkByUserPropertiesCommand = new AsyncRelayCommand(CreateNetworkByUserProperties);
            OpenAddGroupWindowCommand = new RelayCommand(OpenAddGroupWindow);
            RemoveDeviceGroupCommand = new RelayCommand(obj => RemoveDeviceGroup(obj));
            OpenConnectGroupWindowCommand = new RelayCommand(obj => OpenConnectGroupWindow(obj));
            DisconnectFromGroupCommand = new RelayCommand(obj => DisconnectFromGroup(obj));
        }
        private void RemoveDeviceGroup(object obj)
        {
            if (obj is DeviceGroup group)
            {
                DeviceGroups.Remove(group);
            }
        }
        private void DisconnectFromGroup(object obj)
        {
            if (obj is DeviceAllocation deviceAllocationDto)
            {
                var existGroup = DeviceGroups.Where(x => x.Id == deviceAllocationDto.Group.Id).FirstOrDefault();
                if(existGroup != null)
                {
                    deviceAllocationDto.Device.Allocations.Remove(deviceAllocationDto);
                    existGroup.GroupAllocations.Remove(deviceAllocationDto);
                }
            }
        }
        private void OpenConnectGroupWindow(object obj)
        {
            if (obj is AvailableDevicesForEditorDto deviceDto)
            {
                _windowNavigator.ShowModalView<ConnectToGroupWindow, ConnectToGroupWindowViewModel>(vm =>
                {
                    vm.DeviceGroups = this.DeviceGroups;
                    vm.TriedConnectDevice = deviceDto;
                    vm.DeviceConnected += (result) =>
                    {
                        var (group, device) = result;

                        var existing = device.Allocations.FirstOrDefault(a => a.Group.Id == group.Id);
                        if (existing == null)
                        {
                            var newAllocation = new DeviceAllocation
                            {
                                
                                Group = group,
                                Device = device,
                                Count = 1
                            };           
                            device.Allocations.Add(newAllocation);
                            group.GroupAllocations.Add(newAllocation);
                            return true;
                        }
                        return false;
                    };
                });
            }
        }

        private void OpenAddGroupWindow()
        {
            _windowNavigator.ShowModalView<AddNewGroupWindow, AddNewGroupWindowViewModel>(vm =>
            {
                vm.GroupCreated += (newGroup) =>
                {
                    var existModel = DeviceGroups.Where(x => x.GroupName == vm.GroupName).FirstOrDefault();
                    if(existModel == null) { DeviceGroups.Add(newGroup); return true; }
                    else { return false; }
                };
            });
        }
        private async Task CreateNetworkByUserProperties()
        {
            var Convertedbudget = Budget * 1000;
            var devicesToCreate = AvailableDevices.Where(d => d.Count > 0).ToList();      
            GenerationRequested?.Invoke(devicesToCreate, Convertedbudget);
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
            var limits = new Dictionary<DeviceType, int> 
            {
                { DeviceType.PC, 200 },
                { DeviceType.Router, 30 },
                { DeviceType.Switch, 30 },
                { DeviceType.Printer, 30 },
                { DeviceType.AccessPoint, 30 },
                { DeviceType.Server, 30 }
            };

            foreach (var entry in limits)
            {
                AvailableDevices.Add(new AvailableDevicesForEditorDto
                {
                    DeviceName = entry.Key.ToString(),
                    Type = entry.Key,
                    MaxCount = entry.Value,
                    DeviceSubMenuViewModel = entry.Key switch
                    {
                        DeviceType.PC => new PcSubMenuViewModel(),
                        _ => null
                    },
                    HasCount = entry.Key switch
                    {
                        DeviceType.Router => false,
                        DeviceType.Switch => false,
                        _ => true
                    },
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
