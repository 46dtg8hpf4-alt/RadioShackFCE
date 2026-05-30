// Recibe peticiones HTTP desde Swagger, EndPoints de la API, llama a Service y devuelve respuestas

using Microsoft.AspNetCore.Mvc;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Crear objeto service
        private ProductService productService;

        public ProductsController(ProductService productServiceFromProgram)
        {
            productService = productServiceFromProgram;
        }

        // GET ALL PRODUCTS
        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {

            // Pedir lista al service
            List<Product> products = productService.GetProducts();

            // Devolver lista
            return Ok(products);
        }

        // GET PRODUCT BY ID
        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            // Buscar producto usando service
            Product foundProduct = productService.GetProductById(id);

            // Si no existe
            if (foundProduct == null)
            {
                ApiError error = new ApiError();

                error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                error.Title = "Not Found";
                error.Status = 404;
                error.Detail = "El recurso solicitado no fue encontrado.";
                error.Instance = HttpContext.Request.Path;
                error.ErrorCode = "PRD-001";
                error.ErrorMessage = "Producto no encontrado.";

                return NotFound(error);
            }

            // Si existe
            return Ok(foundProduct);
        }

        // CREATE PRODUCT
        [HttpPost]
        public ActionResult<Product> CreateProduct(Product newProduct)
        {
            // Agregar producto usando service
            productService.CreateProduct(newProduct);

            // Devolver producto creado
            return Created("", newProduct);
        }

        // UPDATE PRODUCT
        [HttpPut("{id}")]
        public ActionResult<Product> UpdateProduct(int id, Product updatedProduct)
        {
            // Actualizar producto usando service
            Product foundProduct = productService.UpdateProduct(id, updatedProduct);

            // Si no existe
            if (foundProduct == null)
            {
                ApiError error = new ApiError();

                error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                error.Title = "Not Found";
                error.Status = 404;
                error.Detail = "El recurso solicitado no fue encontrado.";
                error.Instance = HttpContext.Request.Path;
                error.ErrorCode = "PRD-001";
                error.ErrorMessage = "Producto no encontrado.";

                return NotFound(error);
            }

            // Devolver producto actualizado
            return Ok(foundProduct);
        }

        // DELETE PRODUCT
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            // Eliminar producto usando service
            bool deleted = productService.DeleteProduct(id);

            // Si no existe
            if (deleted == false)
            {
                ApiError error = new ApiError();

                error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                error.Title = "Not Found";
                error.Status = 404;
                error.Detail = "El recurso solicitado no fue encontrado.";
                error.Instance = HttpContext.Request.Path;
                error.ErrorCode = "PRD-001";
                error.ErrorMessage = "Producto no encontrado.";

                return NotFound(error);
            }

            // Eliminado correctamente
            return NoContent();
        }
    }
}