using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecksApp.Helper
{
    public class MemoryHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var allocatedBytes = GC.GetTotalMemory(forceFullCollection: false);
            var threshold = 1024L * 1024L * 1024L; // 1 GB threshold

            var data = new Dictionary<string, object>
            {
                { "AllocatedBytes", allocatedBytes },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) }
            };

            var status = allocatedBytes < threshold ? HealthStatus.Healthy : HealthStatus.Unhealthy;
            return Task.FromResult(new HealthCheckResult(status, description: $"Memory allocated: {allocatedBytes} bytes.", data: data));
        }
    }
}

