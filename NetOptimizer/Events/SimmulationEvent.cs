using NetOptimizer.Interfaces;
using NetOptimizer.Models;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Events
{
    public class SimmulationEvent
    {
        // ID конкретного animation/event
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Packet Packet { get; set; }

        public string FromDeviceId { get; set; }
        public string ToDeviceId { get; set; }
        public string FromPortId { get; set; }
        public string ToPortId { get; set; }


        public string InPortId { get; set; }
        public TaskCompletionSource<bool> AnimationCompleted { get; set; }
            = new();
    }
}
