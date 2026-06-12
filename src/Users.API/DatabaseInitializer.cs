using Microsoft.Data.Sqlite;
using Dapper;

namespace Users.API
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS users (
                    Id TEXT PRIMARY KEY,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FechaRegistro TEXT NOT NULL,
                    Activo INTEGER NOT NULL DEFAULT 1,
                    IntentosFallidos INTEGER NOT NULL DEFAULT 0
                );
            """);
        }
    }
}
