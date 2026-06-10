using System.ComponentModel.DataAnnotations;

namespace Products.API.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Range(1, 999999)]
        public decimal Precio { get; set; }

        [Range(0, 9999)]
        public int Stock { get; set; }

        [Required]
        public string Categoria { get; set; }

        [Required]
        public string Marca { get; set; }

        public CreateProductRequest()
        {
            Nombre = "";
            Descripcion = "";
            Categoria = "";
            Marca = "";
        }
    }
}