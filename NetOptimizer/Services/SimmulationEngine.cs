using NetOptimizer.Events;
using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.DeviceModels;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NetOptimizer.Services
{
    public class SimulationEngine : ISimulationEngine
    {
        private Scenario _currentScenario;

        private readonly Queue<SimmulationEvent> _queue = new();
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
        public void AddAction(ScenarioAction action)
        {
            _currentScenario.Actions.Add(action);
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

                var sourceDevice = source.LogicDevice;
                var targetDevice = target.LogicDevice;

                var sourceL3 = sourceDevice.GetInterfaces()
                    .OfType<ILayer3Interface>()
                    .FirstOrDefault();

                var targetL3 = targetDevice.GetInterfaces()
                    .OfType<ILayer3Interface>()
                    .FirstOrDefault();

                if (sourceL3 == null || targetL3 == null)
                    continue;

                Packet packet = action.PacketType switch
                {
                    PacketType.ICMP => new IcmpPacket
                    {
                        Id = Guid.NewGuid().ToString(),
                        SourceIp = sourceL3.IpV4Address,
                        DestinationIp = targetL3.IpV4Address,
                        TTL = 20,
                        IcmpType = IcmpType.EchoRequest,
                        Sequence = 1
                    },

                    _ => throw new NotSupportedException()
                };

                var entryEvents = sourceDevice.ProcessPacket(packet, null);

                foreach (var ev in entryEvents)
                    _queue.Enqueue(ev);
            }
            Task.Run(ProcessQueue);
        }

        private async Task ProcessQueue()
        {
            while (_queue.Count > 0)
            {
                var currentBatch = new List<SimmulationEvent>();

                while (_queue.Count > 0)
                {
                    currentBatch.Add(_queue.Dequeue());
                }

                var animationTasks = new List<Task>();

                foreach (var ev in currentBatch)
                {
                    if (ev.Packet == null)
                        continue;

                    var key = $"{ev.Packet.Id}-{ev.ToDeviceId}-{ev.ToPortId}";

                    if (!_visited.Add(key))
                        continue;

                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        EventChain.Add(ev);
                    });

                    animationTasks.Add(ev.AnimationCompleted.Task);
                }

                await Task.WhenAll(animationTasks);

                foreach (var ev in currentBatch)
                {
                    var nextDevice = _deviceRegistry.GetById(ev.ToDeviceId);

                    if (nextDevice == null)
                        continue;

                    var newEvents = nextDevice.LogicDevice.ProcessPacket(
                        ev.Packet,
                        ev.ToPortId
                    );

                    foreach (var newEv in newEvents)
                    {
                        newEv.InPortId = newEv.FromPortId;
                        _queue.Enqueue(newEv);
                    }
                }
            }
        }
    }
}
