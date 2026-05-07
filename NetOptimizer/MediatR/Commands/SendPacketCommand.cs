using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Commands
{
    public class SendPacketCommand : IRequest
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
    }
}
