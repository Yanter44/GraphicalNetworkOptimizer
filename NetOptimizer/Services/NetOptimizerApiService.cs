using NetOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NetOptimizer.Services
{
    public class NetOptimizerApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NetOptimizerApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task GetSwitchByAveragePrice()
        {
            var client = _httpClientFactory.CreateClient(ApiServers.NetOptimizerApi.ToString());
            var response = await client.GetAsync("/Inventory/switches");
        }
    }
}
