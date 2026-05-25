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
        ProductService productService = new ProductService();

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
                return NotFound();
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
                return NotFound();
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
                return NotFound();
            }

            // Eliminado correctamente
            return NoContent();
        }
    }
}