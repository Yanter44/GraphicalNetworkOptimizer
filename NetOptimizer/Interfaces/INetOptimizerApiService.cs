using NetOptimizer.Models.Dtos;

namespace NetOptimizer.Interfaces
{
    public interface INetOptimizerApiService
    {
        Task<List<PcResponceDto>> GetAllPcsAsync();
        Task<List<CommutatorResponceDto>> GetAllSwitchesAsync();
        Task<List<RouterResponceDto>> GetAllRoutersAsync();
    }
}
