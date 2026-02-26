using NetOptimizer.Models.Dtos;

namespace NetOptimizer.Interfaces
{
    public interface INetOptimizerApiService
    {
        Task<List<CommutatorResponceDto>> GetAllSwitchesAsync();
    }
}
