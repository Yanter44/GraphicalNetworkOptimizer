using MediatR;
using NetOptimizer.MediatR.Notifications;
using NetOptimizer.Models.Enums;
using NetOptimizer.ViewModels.MainWindoww;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Handlers
{
    public class InterfaceStateChangedHandler : INotificationHandler<InterfaceStateChangedNotification>
    {
        private readonly CanvasViewModel _canvas;

        public InterfaceStateChangedHandler(CanvasViewModel canvas)
        {
            _canvas = canvas;
        }

        public Task Handle(
            InterfaceStateChangedNotification notification,
            CancellationToken cancellationToken)
        {
            var port = notification.Port;

            foreach (var conn in _canvas.Connections)
            {
                if (conn.SourcePort == port)
                {
                    conn.StartPoint.State =
                        notification.IsUp
                            ? PointConnectionState.Connected
                            : PointConnectionState.Disconnected;
                }

                if (conn.TargetPort == port)
                {
                    conn.EndPoint.State =
                        notification.IsUp
                            ? PointConnectionState.Connected
                            : PointConnectionState.Disconnected;
                }
            }

            return Task.CompletedTask;
        }
    }
}
