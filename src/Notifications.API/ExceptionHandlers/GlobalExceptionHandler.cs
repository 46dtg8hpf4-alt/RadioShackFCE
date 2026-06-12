// Capturan excepciones y devuelven respuestas HTTP

using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.Models;

namespace Notifications.API.ExceptionHandlers
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
            error.Detail = "Error inesperado en el servicio de notificaciones.";
            error.Instance = httpContext.Request.Path;
            error.ErrorCode = "NTF-004";
            error.ErrorMessage = "Error interno al procesar la notificación.";

            await httpContext.Response.WriteAsJsonAsync(error);

            return true;
        }
    }
}