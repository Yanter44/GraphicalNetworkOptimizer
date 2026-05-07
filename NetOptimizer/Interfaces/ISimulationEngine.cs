using NetOptimizer.Events;
using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface ISimulationEngine
    {
        void LoadScenario(Scenario scenario);
        void AddAction(ScenarioAction action);
        void StartSimmulation();
        ObservableCollection<SimmulationEvent> EventChain { get; }
    }
}
