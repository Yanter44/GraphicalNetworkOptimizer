using NetOptimizer.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetOptimizer.Models
{
    public class Scenario
    {
        public string Id = new Guid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<ScenarioAction> Actions { get; set; } = new();
        public ObservableCollection<SimmulationEvent> Events { get; set; } = new();
    }
}
