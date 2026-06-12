using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Serilog;
using Serilog.Events;

namespace Orders.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseAppMiddleware(this WebApplication app)
        {
            // middleware en Componentes_miniapi
            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (httpContext, _, ex) =>
                    (ex != null || httpContext.Response.StatusCode >= 500) ? LogEventLevel.Error :
                    (httpContext.Response.StatusCode >= 400) ? LogEventLevel.Warning :
                    (httpContext.Request.Path.StartsWithSegments("/health"))
                        ? LogEventLevel.Verbose : LogEventLevel.Information;
            });

            // Endpoint JSON con estado detallado
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Liveness
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("api"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Readiness
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("database"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Dashboard web
            app.MapHealthChecksUI(setup => setup.UIPath = "/health-ui");

            return app;
        }
    }
}