using BudgetApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BudgetApp.Infrastructure.Security
{
    public class TinkClient : ITinkClient

    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TinkClient(HttpClient httpClient, IConfiguration configuration)

        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
    }
}
