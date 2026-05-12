using MediatR;
using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.MediatR.Commands
{
    public class SendPacketCommand : IRequest
    {
        public PacketType PacketType { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
    }
}
