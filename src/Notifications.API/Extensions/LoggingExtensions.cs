using Serilog;
using Serilog.Events;

namespace Notifications.API.Extensions
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