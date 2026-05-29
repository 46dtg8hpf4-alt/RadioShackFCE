// Capturan excepciones y devuelven respuestas HTTP

using Microsoft.AspNetCore.Diagnostics;

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

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    Message = exception.Message
                });

            return true;
        }
    }
}