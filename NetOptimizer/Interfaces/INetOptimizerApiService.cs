using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Interfaces
{
    public interface INetOptimizerApiService
    {
        Task<List<CommutatorResponceDto>> GetAllSwitchesAsync();
    }
}
