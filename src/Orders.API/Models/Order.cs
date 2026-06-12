namespace Orders.API.Models
{
    /// <summary>
    /// Representa una orden de compra en el sistema.
    /// </summary>
    public record Order
    {
        /// <summary>Identificador único de la orden.</summary>
        /// <example>f1e2d3c4-0000-0000-0000-aabbccddeeff</example>
        public Guid Id { get; init; }

        /// <summary>Identificador del usuario que realizó la compra.</summary>
        /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
        public Guid usuarioId { get; init; }

        /// <summary>Lista de ítems incluidos en la orden.</summary>
        public List<OrderItem> Items { get; init; } = new();

        /// <summary>Costo total calculado de la orden.</summary>
        /// <example>3000.00</example>
        public decimal Total { get; init; }

        /// <summary>Estado actual de la orden (Pendiente, Confirmada, Entregada, Cancelada).</summary>
        /// <example>Pendiente</example>
        public string Estado { get; init; } = string.Empty;

        /// <summary>Fecha y hora de creación de la orden.</summary>
        /// <example>2024-03-10T11:00:00Z</example>
        public DateTime FechaCreacion { get; init; }
    }

    /// <summary>
    /// Representa un ítem dentro de una orden de compra.
    /// </summary>
    public record OrderItem
    {
        /// <summary>Identificador único del producto comprado.</summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid ProductoId { get; init; } 

        /// <summary>Cantidad de unidades compradas.</summary>
        /// <example>2</example>
        public int Cantidad { get; init; }           

        /// <summary>Precio unitario del producto al momento de la compra.</summary>
        /// <example>1500.00</example>
        public decimal PrecioUnitario { get; init; }   
    }

    /// <summary>
    /// Petición para crear una nueva orden de compra.
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario que realiza la compra. Ejemplo: a1b2c3d4-0000-0000-0000-111122223333</param>
    /// <param name="Items">Lista de productos a comprar con sus respectivas cantidades.</param>
    public record CreateOrderRequest(Guid usuarioId, List<CreateOrderItemRequest> Items);

    /// <summary>
    /// Petición de un ítem para crear la orden.
    /// </summary>
    /// <param name="ProductoId">Identificador único del producto. Ejemplo: 3fa85f64-5717-4562-b3fc-2c963f66afa6</param>
    /// <param name="Cantidad">Cantidad a comprar del producto. Ejemplo: 2</param>
    public record CreateOrderItemRequest(Guid ProductoId, int Cantidad);

    /// <summary>
    /// Petición para actualizar el estado de una orden.
    /// </summary>
    /// <param name="Estado">Nuevo estado a asignar a la orden. Ejemplo: Confirmada</param>
    public record UpdateOrderStatusRequest(string Estado);

    /// <summary>
    /// Respuesta generada al actualizar exitosamente el estado de una orden.
    /// </summary>
    /// <param name="Id">Identificador de la orden modificada. Ejemplo: f1e2d3c4-0000-0000-0000-aabbccddeeff</param>
    /// <param name="Estado">Nuevo estado asignado. Ejemplo: Confirmada</param>
    /// <param name="FechaActualizacion">Fecha y hora de actualización. Ejemplo: 2024-03-10T12:00:00Z</param>
    public record UpdateOrderStatusResponse(Guid Id, string Estado, DateTime FechaActualizacion);
}
