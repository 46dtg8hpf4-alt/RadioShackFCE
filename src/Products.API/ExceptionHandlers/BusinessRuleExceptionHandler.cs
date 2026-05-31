using Microsoft.AspNetCore.Diagnostics;
using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.ExceptionHandlers
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not BusinessRuleException businessException)
            {
                return false;
            }

            httpContext.Response.StatusCode = 409;

            ApiError error = new ApiError();

            error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.9";
            error.Title = "Conflict";
            error.Status = 409;
            error.Detail = "Ya existe un recurso con esos datos.";
            error.Instance = httpContext.Request.Path;
            error.ErrorCode = businessException.ErrorCode;
            error.ErrorMessage = businessException.Message;

            await httpContext.Response.WriteAsJsonAsync(error);

            return true;
        }
    }
}