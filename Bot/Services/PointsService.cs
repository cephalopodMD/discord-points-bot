using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Bot.Services
{
    public class PointsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PointsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> IsPlayerTimedOut(string playerId, string source)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var playerTimedOut = await httpClient.GetAsync($"{_configuration["QueryBaseEndpoint"]}timeout/{source}/{playerId}?code={_configuration["QueryKey"]}");

            var timedOut = await playerTimedOut.Content.ReadAsStringAsync();
            return Boolean.Parse(timedOut);
        }
    }
}