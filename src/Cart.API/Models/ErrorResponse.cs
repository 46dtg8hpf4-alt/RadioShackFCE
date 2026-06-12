namespace Cart.API.Models
{
    /// <summary>
    /// estructura para devolver los errores
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>tipo de error segun rfc 7231.</summary>
        /// <example>https://tools.ietf.org/html/rfc7231#section-6.5.4</example>
        public string type { get; set; } = string.Empty;

        /// <summary>el titulo cortito.</summary>
        /// <example>Not Found</example>
        public string title { get; set; } = string.Empty;

        /// <summary>el numero de error ej 404.</summary>
        /// <example>404</example>
        public int status { get; set; }

        /// <summary>q paso exactamente.</summary>
        /// <example>El recurso solicitado no fue encontrado.</example>
        public string detail { get; set; } = string.Empty;

        /// <summary>en q endpoint fallo.</summary>
        /// <example>/api/cart/a1b2c3d4</example>
        public string instance { get; set; } = string.Empty;

        /// <summary>el codiguito nuestro tipo crt-001.</summary>
        /// <example>CRT-001</example>
        public string errorCode { get; set; } = string.Empty;

        /// <summary>mensaje nuestro de error.</summary>
        /// <example>Carrito no encontrado.</example>
        public string errorMessage { get; set; } = string.Empty;
    }
}
