
namespace NetOptimizer.Models.Dtos
{
    public class CommutatorResponceDto
    {
        public string Vendor { get; set; }
        public string Model { get; set; }

        public int Layer { get; set; }
        public int TotalPorts { get; set; }

        public int SfpPorts { get; set; }

        public bool SupportsPoe { get; set; }

        public decimal AveragePrice { get; set; }
    }
}
