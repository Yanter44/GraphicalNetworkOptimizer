using MediatR;
using NetOptimizer.MediatR.Commands;
using NetOptimizer.ViewModels.MainWindoww;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Handlers
{
    public class CreateDeviceHandler : IRequestHandler<CreateDeviceCommand>
    {
        private readonly CanvasViewModel _canvas;

        public CreateDeviceHandler(CanvasViewModel canvas)
        {
            _canvas = canvas;
        }

        public Task Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            _canvas.OnDeviceCreated(request.Device, request.Settings);
            return Task.CompletedTask;
        }
    }
}
