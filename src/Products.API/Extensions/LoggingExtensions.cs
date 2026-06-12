using Serilog;
using Serilog.Events;

namespace Products.API.Extensions
{
    public static class LoggingExtensions
    {
        public static void AddAppLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()

                .MinimumLevel.Information()

                .MinimumLevel.Override(
                    "Microsoft",
                    LogEventLevel.Warning)
                .MinimumLevel.Override(
                    "Microsoft.Hosting.Lifetime",
                    LogEventLevel.Information)

                .Enrich.FromLogContext()

                .WriteTo.Console()

                .WriteTo.File(
                    "logs/products.log",
                    rollingInterval: RollingInterval.Day)

                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}