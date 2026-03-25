using NetOptimizer.Interfaces;
using NetOptimizer.Models.Dtos;

namespace NetOptimizer.Services
{
    public class DeviceCatalogService
    {
        private readonly INetOptimizerApiService _apiService;
        public List<CommutatorResponceDto> AvailableSwitches { get; private set; } = new();
        public List<RouterResponceDto> AvailableRouters { get; private set; } = new();
        public List<PcResponceDto> AvailablePcs { get; private set; } = new();
        public List<PrinterResponceDto> AvailablePrinters { get; private set; } = new();
        public DeviceCatalogService(INetOptimizerApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task LoadCatalogAsync()
        {
            var pcs = await _apiService.GetAllPcsAsync();
            var commutators = await _apiService.GetAllSwitchesAsync();
            var routers = await _apiService.GetAllRoutersAsync();
            AvailablePcs = pcs.ToList();
            AvailableSwitches = commutators.ToList();
            AvailableRouters = routers.ToList();
            AvailablePrinters.Add(new PrinterResponceDto()   // <--- добавляю принтер лишь для тестов
            {
                Model = "Epson",
                Ports = new List<PortDto>() 
                {
                    new PortDto() { Type = Enums.PortType.RJ45, Count = 1, Speed = "1Гбит/с", SupportsPoe = false}
                },
                AveragePrice = 11000,
            });
        }

    }
}
