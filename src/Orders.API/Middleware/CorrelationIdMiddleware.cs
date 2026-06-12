using Serilog.Context;

namespace Orders.API.Middleware
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
            if (!context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // Guardar el Correlation ID durante toda la request
            context.Items["CorrelationId"] = correlationId.ToString();

            // Devolverlo al cliente en el header
            context.Response.Headers["X-Correlation-Id"] = correlationId.ToString();

            // Agregarlo al contexto de Serilog
            using (LogContext.PushProperty("CorrelationId", correlationId.ToString()))
            {
                await _next(context);
            }
        }
    }
}
