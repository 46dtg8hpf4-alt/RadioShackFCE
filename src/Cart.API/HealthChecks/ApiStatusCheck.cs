using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cart.API.HealthChecks
{
    public class ApiStatusCheck : IHealthCheck
    {
        private static readonly DateTime _startTime = Process.GetCurrentProcess().StartTime;

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var uptime = DateTime.Now - _startTime;
            var data = new Dictionary<string, object>
            {
                { "Uptime", uptime.ToString() },
                { "DotNetVersion", RuntimeInformation.FrameworkDescription },
                { "StartTime", _startTime.ToString("o") }
            };

            return Task.FromResult(HealthCheckResult.Healthy("API is running", data));
        }
    }
}
