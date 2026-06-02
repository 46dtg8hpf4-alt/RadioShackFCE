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

            return await conn.QueryAsync<Product>("""
                SELECT *
                FROM Products
                ORDER BY Id
            """);
        }

        // GET PRODUCT BY ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            using var conn = CreateConnection();

            return await conn.QuerySingleOrDefaultAsync<Product>("""
                SELECT *
                FROM Products
                WHERE Id = @id
            """, new { id });
        }

        // CREATE PRODUCT
        public async Task<Product> CreateAsync(Product product)
        {
            using var conn = CreateConnection();

            var id = await conn.ExecuteScalarAsync<int>("""
                INSERT INTO Products
                (
                    Nombre,
                    Descripcion,
                    Precio,
                    Stock,
                    Categoria,
                    Marca
                )
                VALUES
                (
                    @Nombre,
                    @Descripcion,
                    @Precio,
                    @Stock,
                    @Categoria,
                    @Marca
                );

                SELECT last_insert_rowid();
            """, product);

            return (await GetByIdAsync(id))!;
        }

        // UPDATE PRODUCT
        public async Task<bool> UpdateAsync(int id, Product product)
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
        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = CreateConnection();

            var rows = await conn.ExecuteAsync("""
                DELETE FROM Products
                WHERE Id = @id
            """, new { id });

            return rows > 0;
        }
    }
}