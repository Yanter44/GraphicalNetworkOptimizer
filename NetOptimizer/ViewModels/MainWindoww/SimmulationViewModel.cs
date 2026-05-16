using NetOptimizer.Common;
using NetOptimizer.Events;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace NetOptimizer.ViewModels.MainWindoww
{
    public class SimmulationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Scenario> ScenarioList { get; set; }
        private bool _isSimulationRunning;
        private int _simmulationSpeed;
        private SimulationState _state;
        private Scenario _selectedScenario;
        public Scenario SelectedScenario
        {
            get => _selectedScenario;
            set
            {
                _selectedScenario = value;
                OnPropertyChanged();

                AttachScenario(_selectedScenario);

                RebuildEventsViewModel();
                RebuildActionsView();
            }
        }

        public ObservableCollection<ScenarioActionViewModel> ActionsView { get; set; }
        public ObservableCollection<SimulationEventViewModel> Events { get; set; }
        public int SimmulationSpeed { get => _simmulationSpeed; set { if (value == _simmulationSpeed) return; else { _simmulationSpeed = value; OnPropertyChanged(); } } }
        public bool IsSimulationRunning { get => _isSimulationRunning; set { if (_isSimulationRunning == value) return; _isSimulationRunning = value; OnPropertyChanged();} }
        public SimulationState State { get => _state; set { if (_state == value) return; _state = value;OnPropertyChanged(); }}

        private readonly IDeviceRegistry _devicesService;
        private readonly ISimulationEngine _engine;
        public ICommand AddNewScenarioCommand { get; }
        public ICommand RemoveScenarioCommand { get; }
        public ICommand StartSimulationCommand { get; } 
        public ICommand DeleteScenarioActionCommand { get; }
        public ICommand PauseSimulationCommand { get; }
        public ICommand StopSimulationCommand { get; }
        public SimmulationViewModel(ISimulationEngine engine, IDeviceRegistry devicesService)
        {
            _engine = engine;
            _devicesService = devicesService;
            AddNewScenarioCommand = new RelayCommand(AddNewScenario);
            RemoveScenarioCommand = new RelayCommand(RemoveScenario);
            StartSimulationCommand = new RelayCommand(StartSimulation);
            DeleteScenarioActionCommand = new RelayCommand(action => DeleteScenarioAction((ScenarioActionViewModel)action));
            PauseSimulationCommand = new RelayCommand(PauseSimulation);
            StopSimulationCommand = new RelayCommand(StopSimulation);
            Initialize();
        }
        private void ResumeSimulation()
        {
            if (State != SimulationState.Paused)
                return;

            State = SimulationState.Running;
            IsSimulationRunning = true;
            _engine.ResumeSimulation();
        }
        private void StopSimulation()
        {
            State = SimulationState.Stopped;
            IsSimulationRunning = false;

            _engine.StopSimmulation();
        }
        private void PauseSimulation()
        {
            if (State != SimulationState.Running)
                return;

            State = SimulationState.Paused;
            IsSimulationRunning = false;
            _engine.PauseSimmulation();
        }
        private void RebuildEventsViewModel()
        {
            if (SelectedScenario == null)
                return;

            Events = new ObservableCollection<SimulationEventViewModel>(
                SelectedScenario.Events.Select(a =>
                {
                    var fromDevice = _devicesService.GetById(a.FromDeviceId);
                    var toDevice = _devicesService.GetById(a.ToDeviceId);

                    return new SimulationEventViewModel
                    {
                        From = fromDevice?.LogicDevice.Name ?? a.FromDeviceId,
                        To = toDevice?.LogicDevice.Name ?? a.ToDeviceId,
                        Type = a.Packet switch
                        {
                            IcmpPacket => "ICMP",
                            ArpPacket => "ARP",
                            _ => a.Packet.GetType().Name
                        }
                    };
                })
            );
            OnPropertyChanged(nameof(Events));
        }
        private void AttachScenario(Scenario scenario)
        {
            scenario.Events.CollectionChanged += (_, __) =>
            {
                RebuildEventsViewModel();
            };

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
                    Id = a.Id,
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
            _engine.LoadScenario(SelectedScenario);
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
        private void DeleteScenarioAction(ScenarioActionViewModel action)
        {
            var toRemove = SelectedScenario.Actions
                .Where(x => x.Id == action.Id )
                .ToList();

            foreach (var item in toRemove)
            {
                SelectedScenario.Actions.Remove(item);
            }
        }
        public void AddActionToSelectedScenario(ScenarioAction action)
        {
            if (SelectedScenario == null)
                return;

            SelectedScenario.Actions.Add(action);
        }
        private void StartSimulation()
        {
            if (State == SimulationState.Paused)
            {
                ResumeSimulation();
                return;
            }

            if (State == SimulationState.Running)
                return;

            State = SimulationState.Running;
            IsSimulationRunning = true;

            _engine.LoadScenario(SelectedScenario);
            _engine.StartSimmulation(SimmulationSpeed);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
