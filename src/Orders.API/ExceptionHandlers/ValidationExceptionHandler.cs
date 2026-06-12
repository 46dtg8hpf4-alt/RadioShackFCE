using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException ex) return false;

        context.Response.StatusCode = 400; 

        var correlationId = context.Items.TryGetValue("CorrelationId", out var id) ? id?.ToString() : null;

        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Bad Request",
            status = 400,
            detail = "Ocurrió un error de validación en los datos de entrada.",
            instance = context.Request.Path.Value,
            errorCode = ex.ErrorCode, 
            errorMessage = ex.Message,
            correlationId = correlationId
        }, cancellationToken: cancellationToken);

        return true;
    }
}