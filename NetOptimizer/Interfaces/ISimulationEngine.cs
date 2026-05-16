using NetOptimizer.Events;
using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface ISimulationEngine
    {
        void LoadScenario(Scenario scenario);
        void AddAction(ScenarioAction action);
        void StartSimmulation(int simmulationSpeed);
        void StopSimmulation();
        void PauseSimmulation();
        void ResumeSimulation();
        int SimmulationSpeed { get; }
        event NotifyCollectionChangedEventHandler SimulationEventsChanged;
    }
}
