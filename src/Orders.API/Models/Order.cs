namespace Orders.API.Models
{

    public record Order
    {
        public Guid Id { get; init; }
        public Guid usuarioId { get; init; }
        public List<OrderItem> Items { get; init; } = new();
        public decimal Total { get; init; }
        public string Estado { get; init; } = string.Empty;
        public DateTime FechaCreacion { get; init; }
    }

    public record OrderItem
    {
        public Guid ProductoId { get; init; } 
        public int Cantidad { get; init; }           
        public decimal PrecioUnitario { get; init; }   
    }

    public record CreateOrderRequest
    (
        Guid usuarioId,
        List<CreateOrderItemRequest> Items
    );

    public record CreateOrderItemRequest
    (
        Guid ProductoId,
        int Cantidad
    );

    public record UpdateOrderStatusRequest(string Estado);

    public record UpdateOrderStatusResponse(Guid Id, string Estado, DateTime FechaActualizacion);
}
