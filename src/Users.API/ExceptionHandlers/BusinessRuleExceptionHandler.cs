using Microsoft.AspNetCore.Diagnostics;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not BusinessRuleException ex)
                return false;

            int statusCode = ex.ErrorCode switch
            {
                "USR-001" => StatusCodes.Status409Conflict,       
                "USR-002" => StatusCodes.Status400BadRequest,     
                "USR-003" => StatusCodes.Status401Unauthorized,   
                "USR-004" => StatusCodes.Status403Forbidden,
                "USR-005" => StatusCodes.Status403Forbidden, 
                _ => StatusCodes.Status400BadRequest
            };

            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                type = "https://tools.ietf.org/html/rfc7231",
                title = "Error",
                status = statusCode,
                detail = "Ocurrió un error al procesar las reglas de negocio del usuario.",
                instance = context.Request.Path.Value,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}
