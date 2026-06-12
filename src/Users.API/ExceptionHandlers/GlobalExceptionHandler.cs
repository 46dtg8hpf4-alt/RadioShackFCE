using Microsoft.AspNetCore.Diagnostics;

namespace Users.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error interno no controlado en Users.API");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var correlationId = context.Items["CorrelationId"]?.ToString();

            var errorResponse = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "Internal Server Error",
                status = StatusCodes.Status500InternalServerError,
                detail = "Ocurrió un error inesperado en el servidor.",
                instance = context.Request.Path.Value,
                errorCode = "USR-006",
                errorMessage = "Error interno al procesar el usuario.",
                correlationId = correlationId
            };

            await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}
