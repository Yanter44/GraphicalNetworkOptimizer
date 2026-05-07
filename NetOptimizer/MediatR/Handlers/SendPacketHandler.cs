using MediatR;
using NetOptimizer.MediatR.Commands;
using NetOptimizer.Models;
using NetOptimizer.Models.Enums;
using NetOptimizer.ViewModels.MainWindoww;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Handlers
{
    public class SendPacketHandler : IRequestHandler<SendPacketCommand>
    {
        private readonly SimmulationViewModel _simVM;

        public SendPacketHandler(SimmulationViewModel simVM)
        {
            _simVM = simVM;
        }

        public Task Handle(SendPacketCommand request, CancellationToken cancellationToken)
        {
            _simVM.AddActionToSelectedScenario(new ScenarioAction
            {
                PacketType = PacketType.ICMP,
                SourceDeviceId = request.SourceId,
                TargetDeviceId = request.TargetId
            });

            return Task.CompletedTask;
        }
    }
}
