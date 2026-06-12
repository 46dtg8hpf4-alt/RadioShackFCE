namespace Cart.API.Models
{
    /// <summary>
    /// modelo del carrito.
    /// </summary>
    public record Cart
    {
        /// <summary>id del usuario dueño.</summary>
        /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
        public Guid UsuarioId { get; init; }

        /// <summary>los productos adentro del carrito.</summary>
        public CartItem[] Items { get; init; } = Array.Empty<CartItem>();

        /// <summary>cuando fue la ultima vez q se toco.</summary>
        /// <example>2024-03-10T10:45:00Z</example>
        public DateTime FechaActualizacion { get; init; }
    }

    /// <summary>
    /// un item del carrito
    /// </summary>
    public record CartItem
    {
        /// <summary>id del producto de la otra api.</summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid ProductoId { get; init; }

        /// <summary>cuantos quiere llevar.</summary>
        /// <example>2</example>
        public int Cantidad { get; init; }
    }

    /// <summary>
    /// dto para meter un producto
    /// </summary>
    /// <param name="ProductoId">Identificador del producto a agregar.</param>
    /// <param name="Cantidad">Cantidad a agregar.</param>
    public record AddCartItemRequest(Guid ProductoId, int Cantidad);

    /// <summary>
    /// dto para cambiar la cantidad
    /// </summary>
    /// <param name="Cantidad">Nueva cantidad del producto.</param>
    public record UpdateCartItemRequest(int Cantidad);
}
