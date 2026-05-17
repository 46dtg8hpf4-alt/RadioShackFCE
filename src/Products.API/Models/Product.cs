namespace Products.API.Models
{
    public class Product
    {
        // Id único del producto
        public int Id { get; set; }

        // Nombre del producto
        public string Nombre { get; set; }

        // Descripción del producto
        public string Descripcion { get; set; }

        // Precio del producto
        public decimal Precio { get; set; }

        // Cantidad disponible en stock
        public int Stock { get; set; }

        // Categoría del producto
        public string Categoria { get; set; }

        // Marca del producto
        public string Marca { get; set; }

        // Constructor vacío
        public Product()
        {
            Nombre = "";
            Descripcion = "";
            Categoria = "";
            Marca = "";
        }
    }
}