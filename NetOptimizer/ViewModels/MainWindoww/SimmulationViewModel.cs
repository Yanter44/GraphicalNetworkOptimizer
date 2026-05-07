using NetOptimizer.Common;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.MainWindoww
{
    public class SimmulationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Scenario> ScenarioList { get; set; }

        private Scenario _selectedScenario;
        public Scenario SelectedScenario
        {
            get => _selectedScenario;
            set
            {
                _selectedScenario = value;
                OnPropertyChanged();

                if (_selectedScenario != null)
                    AttachScenario(_selectedScenario);
                RebuildActionsView();
            }
        }
        public ObservableCollection<ScenarioActionViewModel> ActionsView { get; set; }
        public ObservableCollection<SimulationEventViewModel> Events { get; set; }

        private readonly IDeviceRegistry _devicesService;
        private readonly ISimulationEngine _engine;
        public ICommand AddNewScenarioCommand { get; }
        public ICommand RemoveScenarioCommand { get; }
        public ICommand StartSimmulationCommand { get; }

        public SimmulationViewModel(ISimulationEngine engine, IDeviceRegistry devicesService)
        {
            _engine = engine;
            _engine.EventChain.CollectionChanged += (s, e) =>
            {
                RebuildEventsView();
            };
            _devicesService = devicesService;
            AddNewScenarioCommand = new RelayCommand(AddNewScenario);
            RemoveScenarioCommand = new RelayCommand(RemoveScenario);
            StartSimmulationCommand = new RelayCommand(StartSimulation);
            Initialize();
        }
        private void RebuildEventsView()
        {
            Events = new ObservableCollection<SimulationEventViewModel>(
                _engine.EventChain.Select(e =>
                {
                    var fromDevice = _devicesService.GetById(e.FromDeviceId);
                    var toDevice = _devicesService.GetById(e.ToDeviceId);

                    return new SimulationEventViewModel
                    {
                        From = fromDevice?.LogicDevice.Name ?? e.FromDeviceId,
                        To = toDevice?.LogicDevice.Name ?? e.ToDeviceId,
                        Type = e.PacketType.ToString()
                    };
                })
            );

            OnPropertyChanged(nameof(Events));
        }
        private void AttachScenario(Scenario scenario)
        {
            scenario.Actions.CollectionChanged += (_, __) =>
            {
                RebuildActionsView();
            };
        }
        private void RebuildActionsView()
        {
            ActionsView = new ObservableCollection<ScenarioActionViewModel>(
                SelectedScenario.Actions.Select(a => new ScenarioActionViewModel
                {
                    SourceDevice = _devicesService.GetById(a.SourceDeviceId),
                    TargetDevice = _devicesService.GetById(a.TargetDeviceId),
                    PacketType = a.PacketType
                })
            );
            OnPropertyChanged(nameof(ActionsView));
        }
        private void Initialize()
        {
            ScenarioList = new ObservableCollection<Scenario>
            {new Scenario { Name = "Scenario 0", Description = "Initial" }};
            SelectedScenario = ScenarioList.First();
        }

        private void AddNewScenario()
        {
            var scenario = new Scenario
            {
                Name = $"Scenario {ScenarioList.Count + 1}",
                Description = "Generated"
            };

            ScenarioList.Add(scenario);
            SelectedScenario = scenario;
        }

        private void RemoveScenario()
        {
            if (SelectedScenario == null)
                return;

            ScenarioList.Remove(SelectedScenario);
        }
        public void AddActionToSelectedScenario(ScenarioAction action)
        {
            if (SelectedScenario == null)
                return;

            SelectedScenario.Actions.Add(action);
        }

        private void StartSimulation()
        {
            _engine.LoadScenario(SelectedScenario);
            _engine.StartSimmulation();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
