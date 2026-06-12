using Microsoft.AspNetCore.Diagnostics;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers;

public class BusinessRuleExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BusinessRuleException ex) return false;

        // CRT-003 = stock insuficiente = 422
        context.Response.StatusCode = 422;

        var correlationId = context.Items.TryGetValue("CorrelationId", out var id) ? id?.ToString() : null;

        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            title = "Unprocessable Entity",
            status = 422,
            detail = "No se puede procesar la solicitud.",
            instance = context.Request.Path.Value,
            errorCode = ex.ErrorCode,
            errorMessage = ex.Message,
            correlationId = correlationId
        }, cancellationToken: cancellationToken);

        return true;
    }
}
