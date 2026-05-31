// Acá se representan los modelos del sistema y validaciones para errores 

using System.ComponentModel.DataAnnotations;

namespace Products.API.Models
{
    public class Product
    {
        // Id único del producto
        public int Id { get; set; }

        // Nombre del producto
        [Required]
        public string Nombre { get; set; }

        // Descripción del producto
        [Required]
        public string Descripcion { get; set; }

        // Precio del producto
        [Range(1, 999999)]
        public decimal Precio { get; set; }

        // Cantidad disponible en stock
        [Range(0, 9999)]
        public int Stock { get; set; }

        // Categoría del producto
        [Required]
        public string Categoria { get; set; }

        // Marca del producto
        [Required]
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