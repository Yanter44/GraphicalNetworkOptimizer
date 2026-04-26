
using NetOptimizer.Models.DeviceModels.SubProperties;
using NetOptimizer.Models.Enums;

namespace NetOptimizer.Models.Dtos
{
    public class CommutatorResponceDto
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public int Layer { get; set; }
        public bool IsManaged { get; set; }
        public List<PortDto> Ports { get; set; }
        public PoeSpecs PoeSpecs { get; set; }
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; }
        public SwitchProtocolSupport ProtocolSupport { get; set; }
        public SwitchRoleType SwitchRoleType { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
