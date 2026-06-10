// Contiene la lógica del negocio, asi controller no hace todo


using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services
{
    public class ProductService
    {
        // Simulación de base de datos en memoria
        private List<Product> products;

        // Constructor
        public ProductService()
        {
            products = new List<Product>();

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
            product2.Descripcion = "Computadora clásica 80s";
            product2.Precio = 1200;
            product2.Stock = 3;
            product2.Categoria = "Computadoras";
            product2.Marca = "Commodore";

            // Producto 3
            Product product3 = new Product();
            product3.Id = 3;
            product3.Nombre = "Atari 2600";
            product3.Descripcion = "Consola retro";
            product3.Precio = 900;
            product3.Stock = 4;
            product3.Categoria = "Gaming";
            product3.Marca = "Atari";

            // Agregar productos
            products.Add(product1);
            products.Add(product2);
            products.Add(product3);
        }

        // GET ALL PRODUCTS
        public List<Product> GetProducts()
        {
            return products;
        }

        // GET PRODUCT BY ID
        public Product? GetProductById(int id)
        {
            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    return product;
                }
            }

            return null;
        }

        // CREATE PRODUCT
        public void CreateProduct(Product newProduct)
        {
            foreach (Product product in products)
            {
                if (product.Nombre == newProduct.Nombre &&
                    product.Categoria == newProduct.Categoria)
                {
                    throw new BusinessRuleException("PRD-003", "Ya existe un producto con ese nombre en la categoría.");
                }
            }

            products.Add(newProduct);
        }

        // UPDATE PRODUCT
        public Product? UpdateProduct(int id, Product updatedProduct)
        {
            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    product.Nombre = updatedProduct.Nombre;
                    product.Descripcion = updatedProduct.Descripcion;
                    product.Precio = updatedProduct.Precio;
                    product.Stock = updatedProduct.Stock;
                    product.Categoria = updatedProduct.Categoria;
                    product.Marca = updatedProduct.Marca;

                    return product;
                }
            }

            return null;
        }

        // DELETE PRODUCT
        public bool DeleteProduct(int id)
        {
            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    products.Remove(product);

                    return true;
                }
            }

            return false;
        }
    }
}