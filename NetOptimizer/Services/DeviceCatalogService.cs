using NetOptimizer.Interfaces;
using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Services
{
    public class DeviceCatalogService
    {
        private readonly INetOptimizerApiService _apiService;
        public List<CommutatorResponceDto> AvailableSwitches { get; private set; } = new();

        public DeviceCatalogService(INetOptimizerApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task LoadCatalogAsync()
        {
            var result = await _apiService.GetAllSwitchesAsync();
            AvailableSwitches = result.ToList();
        }

    }
}
