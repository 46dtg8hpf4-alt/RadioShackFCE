// Contiene la lógica del negocio, así Controller no hace todo

using Products.API.Data;
using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services
{
    public class ProductService
    {
        // Acceso a la base de datos
        private readonly ProductRepository repository;

        // Constructor
        public ProductService(ProductRepository repositoryFromProgram)
        {
            repository = repositoryFromProgram;
        }

        // GET ALL PRODUCTS
        public List<Product> GetProducts()
        {
            return repository.GetAllAsync().Result.ToList();
        }

        // GET PRODUCT BY ID
        public Product? GetProductById(int id)
        {
            return repository.GetByIdAsync(id).Result;
        }

        // CREATE PRODUCT
        public void CreateProduct(Product newProduct)
        {
            List<Product> products = repository.GetAllAsync().Result.ToList();

            // Validar producto duplicado
            foreach (Product product in products)
            {
                if (product.Nombre == newProduct.Nombre &&
                    product.Categoria == newProduct.Categoria)
                {
                    throw new BusinessRuleException(
                        "PRD-003",
                        "Ya existe un producto con ese nombre en la categoría.");
                }
            }

            repository.CreateAsync(newProduct).Wait();
        }

        // UPDATE PRODUCT
        public Product? UpdateProduct(int id, Product updatedProduct)
        {
            Product? existingProduct =
                repository.GetByIdAsync(id).Result;

            if (existingProduct == null)
            {
                return null;
            }

            repository.UpdateAsync(id, updatedProduct).Wait();

            return repository.GetByIdAsync(id).Result;
        }

        // DELETE PRODUCT
        public bool DeleteProduct(int id)
        {
            Product? existingProduct =
                repository.GetByIdAsync(id).Result;

            if (existingProduct == null)
            {
                return false;
            }

            return repository.DeleteAsync(id).Result;
        }
    }
}