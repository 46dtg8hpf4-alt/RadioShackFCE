using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Runtime.InteropServices;

namespace Users.API.HealthChecks
{
    public class ApiStatusCheck : IHealthCheck
    {
        private static readonly DateTime _startupTime = DateTime.UtcNow;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>
            {
                { "Uptime", (DateTime.UtcNow - _startupTime).ToString() },
                { "DotNetVersion", RuntimeInformation.FrameworkDescription },
                { "StartTime", _startupTime.ToString("o") }
            };

            return Task.FromResult(HealthCheckResult.Healthy("La API está operativa", data));
        }
    }
}
