using NetOptimizer.Enums;
using NetOptimizer.Interfaces;
using NetOptimizer.Models.Dtos;
using Newtonsoft.Json;
using System.Net.Http;

namespace NetOptimizer.Services
{
    public class NetOptimizerApiService : INetOptimizerApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NetOptimizerApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<RouterResponceDto>> GetAllRoutersAsync()
        {
            var client = _httpClientFactory.CreateClient(ApiServers.NetOptimizerApi.ToString());
            var response = await client.GetAsync("/Inventory/GetAllRouters");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<RouterResponceDto>>(json);
            }
            return new List<RouterResponceDto>();
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
        public async Task<List<PcResponceDto>> GetAllPcsAsync()
        {
            var client = _httpClientFactory.CreateClient(ApiServers.NetOptimizerApi.ToString());
            var response = await client.GetAsync("/Inventory/GetAllPcs");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PcResponceDto>>(json);
            }
            return new List<PcResponceDto>();
        }
    }
}
