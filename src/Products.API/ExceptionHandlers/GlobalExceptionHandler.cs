// Capturan excepciones y devuelven respuestas HTTP

using Microsoft.AspNetCore.Diagnostics;
using Products.API.Models;

namespace Products.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = 500;

            ApiError error = new ApiError();

            error.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            error.Title = "Internal Server Error";
            error.Status = 500;
            error.Detail = "Error inesperado en servicio o persistencia.";
            error.Instance = httpContext.Request.Path;
            error.ErrorCode = "PRD-005";
            error.ErrorMessage = "Error interno al procesar el producto.";

            await httpContext.Response.WriteAsJsonAsync(error);

            return true;
        }
    }
}