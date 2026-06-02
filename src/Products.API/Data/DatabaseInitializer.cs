using Dapper;
using Microsoft.Data.Sqlite;

namespace Products.API.Data
{
    // Crea la base de datos y la tabla Products si no existen
    public class DatabaseInitializer
    {
        private readonly IConfiguration _config;

        public DatabaseInitializer(IConfiguration config)
        {
            _config = config;
        }

        public void Initialize()
        {
            string connectionString =
                _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=products.db";

            using var connection = new SqliteConnection(connectionString);

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS Products
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Descripcion TEXT NOT NULL,
                    Precio REAL NOT NULL,
                    Stock INTEGER NOT NULL,
                    Categoria TEXT NOT NULL,
                    Marca TEXT NOT NULL
                );
            """);
        }
    }
}