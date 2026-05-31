using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class BusinessRuleExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BusinessRuleException ex) return false;

        int statusCode;
        string typeUrl;
        string titleText;
        string detailText;

        //hago un if else para diferenciar entre los dos tipos de errores dentro del mismo handler
        
        if (ex.ErrorCode == "ORD-005")
        {
            statusCode = 422;
            typeUrl = "https://tools.ietf.org/html/rfc4918#section-11.2";
            titleText = "Unprocessable Entity";
            detailText = "No se puede procesar la solicitud.";
        
        }
        
        {
            statusCode = 409;
            typeUrl = "https://tools.ietf.org/html/rfc7231#section-6.5.9";
            titleText = "Conflict";
            detailText = "No se puede modificar el estado.";
        }

        context.Response.StatusCode = statusCode;
        
        await context.Response.WriteAsJsonAsync(new
        {
            type = typeUrl,
            title = titleText,
            status = statusCode,
            detail = detailText,
            instance = context.Request.Path.Value,
            errorCode = ex.ErrorCode,
            errorMessage = ex.Message
        }, cancellationToken: cancellationToken);

        return true;
    }
}