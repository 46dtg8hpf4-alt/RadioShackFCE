using Microsoft.AspNetCore.Mvc;
using Products.API.Models;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {
            // Lista de productos
            List<Product> products = new List<Product>();

            // Producto 1
            Product product1 = new Product(); // SIMULAMOS REGISTROS EN "BASE DE DATOS"
            product1.Id = 1;
            product1.Nombre = "Apple I";
            product1.Descripcion = "Primera computadora construida por Apple!";
            product1.Precio = 5000;
            product1.Stock = 1;
            product1.Categoria = "Computadoras";
            product1.Marca = "Apple";

            // Producto 2
            Product product2 = new Product();
            product2.Id = 2;
            product2.Nombre = "Commodore 64";
            product2.Descripcion = "Computadora de hogar clásica 80s";
            product2.Precio = 1200;
            product2.Stock = 3;
            product2.Categoria = "Computadoras";
            product2.Marca = "Commodore";

            // Producto 3
            Product product3 = new Product();
            product3.Id = 3;
            product3.Nombre = "Atari 2600";
            product3.Descripcion = "Consola de videojuegos de los 80s";
            product3.Precio = 900;
            product3.Stock = 4;
            product3.Categoria = "Gaming";
            product3.Marca = "Atari";

            // Agregar productos a la lista creada antes
            products.Add(product1);
            products.Add(product2);
            products.Add(product3);

            // Devolver la lista
            return Ok(products);
        }
        

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            // Lista de productos
            List<Product> products = new List<Product>();

            // Producto 1
            Product product1 = new Product();
            product1.Id = 1;
            product1.Nombre = "Apple I";
            product1.Descripcion = "Primera computadora construida por Apple!";
            product1.Precio = 5000;
            product1.Stock = 1;
            product1.Categoria = "Computadoras";
            product1.Marca = "Apple";

            // Producto 2
            Product product2 = new Product();
            product2.Id = 2;
            product2.Nombre = "Commodore 64";
            product2.Descripcion = "Computadora de hogar clásica 80s";
            product2.Precio = 1200;
            product2.Stock = 3;
            product2.Categoria = "Computadoras";
            product2.Marca = "Commodore";

            // Agregar productos a la lista
            products.Add(product1);
            products.Add(product2);

            // Variable para guardar producto encontrado
            Product foundProduct = null;

            // Buscar producto por id
            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    foundProduct = product;
                }
            }

            // Si encontró producto
            if (foundProduct != null)
            {
                return Ok(foundProduct);
            }

            // Si no encontró producto
            return NotFound();
        }


        [HttpPost]
        public ActionResult<Product> CreateProduct(Product newProduct)
        {
            // Simulación de creación de producto

            return Created("", newProduct);
        }


        [HttpPut("{id}")]
        public ActionResult<Product> UpdateProduct(int id, Product updatedProduct)
        {
            // Lista de productos
            List<Product> products = new List<Product>();

            // Producto 1
            Product product1 = new Product();
            product1.Id = 1;
            product1.Nombre = "Apple I";
            product1.Descripcion = "Primera computadora construida por Apple!";
            product1.Precio = 5000;
            product1.Stock = 1;
            product1.Categoria = "Computadoras";
            product1.Marca = "Apple";

            // Producto 2
            Product product2 = new Product();
            product2.Id = 2;
            product2.Nombre = "Commodore 64";
            product2.Descripcion = "Computadora clásica";
            product2.Precio = 1200;
            product2.Stock = 3;
            product2.Categoria = "Computadoras";
            product2.Marca = "Commodore";

            // Agregar productos
            products.Add(product1);
            products.Add(product2);

            // Buscar producto
            Product foundProduct = null;

            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    foundProduct = product;
                }
            }

            // Si no existe
            if (foundProduct == null)
            {
                return NotFound();
            }

            // Actualizar datos
            foundProduct.Nombre = updatedProduct.Nombre;
            foundProduct.Descripcion = updatedProduct.Descripcion;
            foundProduct.Precio = updatedProduct.Precio;
            foundProduct.Stock = updatedProduct.Stock;
            foundProduct.Categoria = updatedProduct.Categoria;
            foundProduct.Marca = updatedProduct.Marca;

            // Devolver producto actualizado
            return Ok(foundProduct);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            // Lista de productos
            List<Product> products = new List<Product>();

            // Producto 1
            Product product1 = new Product();
            product1.Id = 1;
            product1.Nombre = "Apple I";

            // Producto 2
            Product product2 = new Product();
            product2.Id = 2;
            product2.Nombre = "Commodore 64";

            // Agregar productos
            products.Add(product1);
            products.Add(product2);

            // Variable para guardar producto encontrado
            Product foundProduct = null;

            // Buscar producto
            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    foundProduct = product;
                }
            }

            // Si no existe
            if (foundProduct == null)
            {
                return NotFound();
            }

            // Eliminar producto
            products.Remove(foundProduct);

            // Devolver respuesta exitosa
            return NoContent();
        }
    }
}