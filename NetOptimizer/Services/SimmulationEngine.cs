using NetOptimizer.Events;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.DeviceModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetOptimizer.Services
{
    public class SimulationEngine : ISimulationEngine
    {
        private Scenario _currentScenario;

        private readonly Queue<(SimmulationEvent ev, Packet packet)> _queue = new();
        public ObservableCollection<SimmulationEvent> EventChain { get; } = new();

        private readonly IDeviceRegistry _deviceRegistry;

        private readonly HashSet<string> _visited = new();

        public SimulationEngine(IDeviceRegistry deviceRegistry)
        {
            _deviceRegistry = deviceRegistry;
        }

        public void LoadScenario(Scenario scenario)
        {
            _currentScenario = scenario;
        }

        public void StartSimmulation()
        {
            EventChain.Clear();
            _queue.Clear();
            _visited.Clear();

            foreach (var action in _currentScenario.Actions)
            {
                var source = _deviceRegistry.GetById(action.SourceDeviceId);
                var target = _deviceRegistry.GetById(action.TargetDeviceId);

                var device = target.LogicDevice as PcDevice;

                var packet = new Packet
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceDeviceId = source.LogicDevice.Id,
                    DestinationIp = device.NetworkConfig.Interfaces[0].IpV4Address,
                    Type = action.PacketType,
                    TTL = 20
                };

                var events = source.LogicDevice.ProcessPacket(packet);

                foreach (var ev in events)
                    _queue.Enqueue((ev, packet));
            }

            Task.Run(ProcessQueue);
        }
        public void AddAction(ScenarioAction action)
        {
            _currentScenario.Actions.Add(action);
        }
        private async Task ProcessQueue()
        {
            while (_queue.Count > 0)
            {
                var (ev, packet) = _queue.Dequeue();

                if (packet.TTL <= 0)
                    continue;

                packet.TTL--;

                var key = $"{ev.PacketId}-{ev.ToDeviceId}";

                if (!_visited.Add(key))
                    continue; 

                App.Current.Dispatcher.Invoke(() =>
                {
                    EventChain.Add(ev);
                });

                var nextDevice = _deviceRegistry.GetById(ev.ToDeviceId);

                if (nextDevice == null)
                    continue;

                var newPacket = new Packet
                {
                    Id = ev.PacketId,
                    SourceDeviceId = ev.FromDeviceId,
                    DestinationIp = packet.DestinationIp,
                    Type = ev.PacketType,
                    TTL = packet.TTL
                };

                var newEvents = nextDevice.LogicDevice.ProcessPacket(newPacket);

                foreach (var newEv in newEvents)
                    _queue.Enqueue((newEv, newPacket));

                await Task.Delay(1); 
            }
        }
    }
}
