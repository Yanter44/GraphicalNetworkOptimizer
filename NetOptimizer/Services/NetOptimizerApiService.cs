using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.AddDeviceSettingsModels;
using NetOptimizer.Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NetOptimizer.Services
{
    public class NetOptimizerApiService : INetOptimizerApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NetOptimizerApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<CommutatorResponceDto>> GetAllSwitchesAsync()
        {
            var client = _httpClientFactory.CreateClient(ApiServers.NetOptimizerApi.ToString());
            var response = await client.GetAsync("/Inventory/GetAllCommutators");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CommutatorResponceDto>>(json);
            }

            return new List<CommutatorResponceDto>(); 
        }
    }
}
