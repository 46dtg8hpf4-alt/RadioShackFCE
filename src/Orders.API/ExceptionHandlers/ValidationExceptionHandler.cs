using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException ex) return false;

        context.Response.StatusCode = 400; 

        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Bad Request",
            status = 400,
            detail = "Ocurrió un error de validación en los datos de entrada.",
            instance = context.Request.Path.Value,
            errorCode = ex.ErrorCode, 
            errorMessage = ex.Message 
        }, cancellationToken: cancellationToken);

        return true;
    }
}