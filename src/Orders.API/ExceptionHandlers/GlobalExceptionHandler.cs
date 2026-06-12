using Microsoft.AspNetCore.Diagnostics;

namespace Orders.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
       
        context.Response.StatusCode = 500;

        var correlationId = context.Items.TryGetValue("CorrelationId", out var id) ? id?.ToString() : null;

        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Internal Server Error",
            status = 500,
            detail = "Ocurrió un error inesperado en el servidor.",
            instance = context.Request.Path.Value,
            errorCode = "ORD-007",
            errorMessage = "Error interno al procesar la orden.",
            correlationId = correlationId
        }, cancellationToken: cancellationToken);

        return true;
    }
}