using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cart.API.HealthChecks
{
    public class SqliteHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config;

        public SqliteHealthCheck(IConfiguration config)
        {
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var connString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";
                using var conn = new SqliteConnection(connString);
                await conn.OpenAsync(cancellationToken);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy("SQLite cart.db is reachable");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("SQLite cart.db is unreachable", ex);
            }
        }
    }
}
