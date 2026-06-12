namespace Cart.API.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
