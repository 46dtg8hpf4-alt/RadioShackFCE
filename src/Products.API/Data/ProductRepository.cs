using Dapper;
using Microsoft.Data.Sqlite;
using Products.API.Models;

namespace Products.API.Data
{
    // Maneja todas las operaciones de base de datos de Products
    public class ProductRepository
    {
        private readonly IConfiguration _config;

        // Constructor
        public ProductRepository(IConfiguration config)
        {
            _config = config;
        }

        // Crear conexión a SQLite
        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(
                _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=products.db");
        }

        // GET ALL PRODUCTS
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var conn = CreateConnection();

            var rows = await conn.QueryAsync<ProductRow>("""
                SELECT *
                FROM Products
                ORDER BY Nombre
            """);

            List<Product> products = new List<Product>();

            foreach (var row in rows)
            {
                Product product = new Product();

                product.Id = Guid.Parse(row.Id);
                product.Nombre = row.Nombre;
                product.Descripcion = row.Descripcion;
                product.Precio = row.Precio;
                product.Stock = row.Stock;
                product.Categoria = row.Categoria;
                product.Marca = row.Marca;

                products.Add(product);
            }

            return products;
        }

        // GET PRODUCT BY ID
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();

            var row = await conn.QuerySingleOrDefaultAsync<ProductRow>("""
                SELECT *
                FROM Products
                WHERE Id = @id
            """, new { id });

            if (row == null)
            {
                return null;
            }

            Product product = new Product();

            product.Id = Guid.Parse(row.Id);
            product.Nombre = row.Nombre;
            product.Descripcion = row.Descripcion;
            product.Precio = row.Precio;
            product.Stock = row.Stock;
            product.Categoria = row.Categoria;
            product.Marca = row.Marca;

            return product;
        }

        // CREATE PRODUCT
        public async Task<Product> CreateAsync(Product product)
        {
            using var conn = CreateConnection();

            product.Id = Guid.NewGuid();

            await conn.ExecuteAsync("""
                INSERT INTO Products
                (
                    Id,
                    Nombre,
                    Descripcion,
                    Precio,
                    Stock,
                    Categoria,
                    Marca
                )
                VALUES
                (
                    @Id,
                    @Nombre,
                    @Descripcion,
                    @Precio,
                    @Stock,
                    @Categoria,
                    @Marca
                )
            """, product);

            return product;
        }

        // UPDATE PRODUCT
        public async Task<bool> UpdateAsync(Guid id, Product product)
        {
            using var conn = CreateConnection();

            var rows = await conn.ExecuteAsync("""
                UPDATE Products
                SET
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Precio = @Precio,
                    Stock = @Stock,
                    Categoria = @Categoria,
                    Marca = @Marca
                WHERE Id = @Id
            """,
            new
            {
                Id = id,
                product.Nombre,
                product.Descripcion,
                product.Precio,
                product.Stock,
                product.Categoria,
                product.Marca
            });

            return rows > 0;
        }

        // DELETE PRODUCT
        public async Task<bool> DeleteAsync(Guid id)
        {
            using var conn = CreateConnection();

            var rows = await conn.ExecuteAsync("""
                DELETE FROM Products
                WHERE Id = @id
            """, new { id });

            return rows > 0;
        }

        // Clase auxiliar para leer filas de SQLite
        private class ProductRow
        {
            public string Id { get; set; } = "";
            public string Nombre { get; set; } = "";
            public string Descripcion { get; set; } = "";
            public decimal Precio { get; set; }
            public int Stock { get; set; }
            public string Categoria { get; set; } = "";
            public string Marca { get; set; } = "";
        }
    }
}