using Serilog.Context;

namespace Cart.API.Middleware
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

            // guardo el correlation id para tenerlo a mano en toda la request
            context.Items["CorrelationId"] = correlationId.ToString();

            // se lo mando de vuelta al cliente en los headers
            context.Response.Headers["X-Correlation-Id"] = correlationId.ToString();

            // lo meto en serilog asi sale en los logs
            using (LogContext.PushProperty("CorrelationId", correlationId.ToString()))
            {
                await _next(context);
            }
        }
    }
}
