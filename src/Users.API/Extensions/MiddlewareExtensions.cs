using Serilog;
using Serilog.Events;

namespace Users.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseAppMiddlewares(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (httpContext, _, ex) =>
                    (ex != null) ? LogEventLevel.Error :
                    (httpContext.Request.Path.StartsWithSegments("/health"))
                        ? LogEventLevel.Verbose : LogEventLevel.Information;
            });
        }
    }
}
