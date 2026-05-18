using Microsoft.AspNetCore.Diagnostics;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers
{
    public class BusinessRuleExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not BusinessRuleException ex)
                return false;

            int statusCode = ex.ErrorCode switch
            {
                "USR-001" => StatusCodes.Status409Conflict,       // Email ya registrado
                "USR-002" => StatusCodes.Status400BadRequest,     // Datos inválidos
                "USR-003" => StatusCodes.Status401Unauthorized,   // Credenciales incorrectas
                "USR-004" or "USR-005" => StatusCodes.Status403Forbidden, // Bloqueado
                _ => StatusCodes.Status400BadRequest
            };

            context.Response.StatusCode = statusCode;

            // 3. Estructura pedida en la seccion 3.1 "Estructura de respuesta de error"
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
