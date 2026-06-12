using Microsoft.Extensions.DependencyInjection;
using Orders.API.HealthChecks;

namespace Orders.API.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Health Checks
            services.AddHealthChecks()
                .AddCheck<SqliteHealthCheck>("sqlite-db", tags: ["database"])
                .AddCheck<ApiStatusCheck>("api-status", tags: ["api"]);

            services.AddHealthChecksUI(setup =>
            {
                setup.SetEvaluationTimeInSeconds(600); // cada 10 minutos
                setup.AddHealthCheckEndpoint("OrdersApi", "/health");
            }).AddInMemoryStorage();

            // Swagger / OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
