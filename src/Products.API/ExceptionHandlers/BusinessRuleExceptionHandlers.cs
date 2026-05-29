using Microsoft.AspNetCore.Diagnostics;
using Products.API.Exceptions;

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

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    ErrorCode = businessException.ErrorCode,
                    Message = businessException.Message
                });

            return true;
        }
    }
}