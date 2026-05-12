using MediatR;
using NetOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Notifications
{
    public class InterfaceStateChangedNotification : INotification
    {
        public Port Port { get; set; }
        public bool IsUp { get; set; }
    }
}
