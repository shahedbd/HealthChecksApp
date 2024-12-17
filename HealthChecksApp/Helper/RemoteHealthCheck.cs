using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecksApp.Helper
{
    public class RemoteHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RemoteHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync("http://erp.netspeedm.com/Account/Login");
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Remote endpoint is healthy.");
                }
                return HealthCheckResult.Unhealthy("Remote endpoint is unhealthy.");
            }
        }
    }
}

