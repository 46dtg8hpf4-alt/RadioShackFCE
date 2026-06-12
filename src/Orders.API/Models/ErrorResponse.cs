namespace Orders.API.Models
{
    /* esto fue generado por IA porque no sabia como hacerlo */
    
    /// <summary>
    /// Estructura estándar para respuestas de error de la API (4xx y 5xx).
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Tipo de error según RFC 7231.
        /// </summary>
        /// <example>https://tools.ietf.org/html/rfc7231#section-6.5.4</example>
        public string type { get; set; } = string.Empty;

        /// <summary>
        /// Título corto del error HTTP.
        /// </summary>
        /// <example>Not Found</example>
        public string title { get; set; } = string.Empty;

        /// <summary>
        /// Código de estado HTTP.
        /// </summary>
        /// <example>404</example>
        public int status { get; set; }

        /// <summary>
        /// Detalle en texto plano del error HTTP.
        /// </summary>
        /// <example>El recurso solicitado no fue encontrado.</example>
        public string detail { get; set; } = string.Empty;

        /// <summary>
        /// Ruta o instancia donde ocurrió el error.
        /// </summary>
        /// <example>/api/orders/99</example>
        public string instance { get; set; } = string.Empty;

        /// <summary>
        /// Código de error de negocio propio del catálogo.
        /// </summary>
        /// <example>ORD-001</example>
        public string errorCode { get; set; } = string.Empty;

        /// <summary>
        /// Mensaje de error de negocio o validación.
        /// </summary>
        /// <example>Orden no encontrada.</example>
        public string errorMessage { get; set; } = string.Empty;
    }
}
