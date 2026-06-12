using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Users.API.HealthChecks;

namespace Users.API.Extensions
{
    public static class HealthChecksExtensions
    {
        public static void AddAppHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<SqliteHealthCheck>("sqlite-db", tags: ["database", "ready"])
                .AddCheck<ApiStatusCheck>("api-status", tags: ["api", "live", "ready"]);

            services.AddHealthChecksUI(setup =>
            {
                setup.SetEvaluationTimeInSeconds(600); // evalúa cada 10 minutos
                setup.AddHealthCheckEndpoint("Users API Liveness", "/health/live");
                setup.AddHealthCheckEndpoint("Users API Readiness", "/health/ready");
            }).AddInMemoryStorage();
        }

        public static void MapAppHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecksUI(setup => setup.UIPath = "/health-ui");
        }
    }
}