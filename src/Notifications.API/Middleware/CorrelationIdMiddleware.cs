using Serilog.Context;

namespace Notifications.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = Guid.NewGuid().ToString();

            // Guardar el Correlation ID durante toda la request
            context.Items["CorrelationId"] = correlationId;

            // Devolverlo al cliente en el header
            context.Response.Headers["X-Correlation-Id"] =
                correlationId;

            // Agregarlo al contexto de Serilog
            using (LogContext.PushProperty(
                "CorrelationId",
                correlationId))
            {
                await _next(context);
            }
        }
    }
}