using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Orders.API.Extensions
{
    public static class LoggingExtensions
    {
        public static void AddAppLogging(this IHostBuilder builder)
        {
            // sacado de documentacion componentes_miniapi
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // agrego withproperty para diferenciar apis en el log.
                .Enrich.WithProperty("Service", "Orders.API")
                
                // CONSOLA: solo errores y mensajes de inicio del servidor
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(le => le.Level >= LogEventLevel.Error || 
                                                  Matching.FromSource("Microsoft.Hosting.Lifetime")(le))
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
                // ARCHIVO: solo requests HTTP (sin /health ni /swagger)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(le => {
                        var esSerilogMiddleware = Matching.FromSource("Serilog.AspNetCore.RequestLoggingMiddleware")(le);
                        if (!esSerilogMiddleware) return false;

                        if (le.Properties.TryGetValue("RequestPath", out var p) && p is Serilog.Events.ScalarValue s && s.Value is string path)
                            return !path.Contains("/health") && !path.Contains("/swagger");

                        return true;
                    })
                    .WriteTo.File(
                        new Serilog.Formatting.Json.JsonFormatter(),
                        path: "logs/audit.log",
                        rollingInterval: RollingInterval.Day))
                .CreateLogger();

            builder.UseSerilog();
        }
    }
}