// Recibe peticiones HTTP desde Swagger, EndPoints de la API, llama a Service y devuelve respuestas

using Microsoft.AspNetCore.Mvc;
using Products.API.Models;
using Products.API.Services;
using Products.API.DTOs;

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
        public ActionResult<List<ProductResponse>> GetProducts()
        {
            // Pedir lista al service
            List<Product> products = productService.GetProducts();

            // Lista de respuesta
            List<ProductResponse> response = new List<ProductResponse>();

            // Convertir cada Product a ProductResponse
            foreach (Product product in products)
            {
                ProductResponse productResponse = new ProductResponse();

                productResponse.Id = product.Id;
                productResponse.Nombre = product.Nombre;
                productResponse.Descripcion = product.Descripcion;
                productResponse.Precio = product.Precio;
                productResponse.Stock = product.Stock;
                productResponse.Categoria = product.Categoria;
                productResponse.Marca = product.Marca;

                response.Add(productResponse);
            }

            // Devolver lista
            return Ok(response);
        }

        // GET PRODUCT BY ID
        [HttpGet("{id}")]
        public ActionResult<ProductResponse> GetProductById(int id)
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

            // Convertir Product a ProductResponse
            ProductResponse response = new ProductResponse();

            response.Id = foundProduct.Id;
            response.Nombre = foundProduct.Nombre;
            response.Descripcion = foundProduct.Descripcion;
            response.Precio = foundProduct.Precio;
            response.Stock = foundProduct.Stock;
            response.Categoria = foundProduct.Categoria;
            response.Marca = foundProduct.Marca;

            // Devolver producto encontrado
            return Ok(response);
        }


        // CREATE PRODUCT
        [HttpPost]
        public ActionResult<ProductResponse> CreateProduct(
            CreateProductRequest request)
        {
            // Convertir DTO a Product
            Product newProduct = new Product();

            newProduct.Nombre = request.Nombre;
            newProduct.Descripcion = request.Descripcion;
            newProduct.Precio = request.Precio;
            newProduct.Stock = request.Stock;
            newProduct.Categoria = request.Categoria;
            newProduct.Marca = request.Marca;

            // Crear producto usando el service
            productService.CreateProduct(newProduct);

            // Convertir Product a DTO de respuesta
            ProductResponse response = new ProductResponse();

            response.Id = newProduct.Id;
            response.Nombre = newProduct.Nombre;
            response.Descripcion = newProduct.Descripcion;
            response.Precio = newProduct.Precio;
            response.Stock = newProduct.Stock;
            response.Categoria = newProduct.Categoria;
            response.Marca = newProduct.Marca;

            // Devolver producto creado
            return Created("", response);
        }

        // UPDATE PRODUCT
        [HttpPut("{id}")]
        public ActionResult<Product> UpdateProduct(
            int id,
            UpdateProductRequest request)
        {
            // Convertir DTO a Product
            Product updatedProduct = new Product();

            updatedProduct.Nombre = request.Nombre;
            updatedProduct.Descripcion = request.Descripcion;
            updatedProduct.Precio = request.Precio;
            updatedProduct.Stock = request.Stock;
            updatedProduct.Categoria = request.Categoria;
            updatedProduct.Marca = request.Marca;

            // Actualizar producto usando service
            Product foundProduct =
                productService.UpdateProduct(id, updatedProduct);

            // Si el producto no existe
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