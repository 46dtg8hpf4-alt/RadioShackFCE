using Cart.API.HealthChecks;

namespace Cart.API.Extensions
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
                setup.AddHealthCheckEndpoint("CartApi", "/health");
            }).AddInMemoryStorage();

            // Swagger / OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
