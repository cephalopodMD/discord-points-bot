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

        public async Task<bool> IsPlayerTimedOut(string playerId)
        {
            var httpclient = _httpClientFactory.CreateClient();
            var playerTimedOut = await httpclient.GetAsync($"{_configuration["QueryBaseEndpoint"]}timeout/{playerId}?code={_configuration["QueryKey"]}");

            var timedOut = await playerTimedOut.Content.ReadAsStringAsync();
            return timedOut == "timedout";
        }
    }
}