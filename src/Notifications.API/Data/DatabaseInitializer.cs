using Dapper;
using Microsoft.Data.Sqlite;

namespace Notifications.API.Data
{
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
                ?? "Data Source=notifications.db";

            using var connection =
                new SqliteConnection(connectionString);

            connection.Execute("""
                CREATE TABLE IF NOT EXISTS Notifications
                (
                    Id TEXT PRIMARY KEY,
                    UsuarioId TEXT NOT NULL,
                    Mensaje TEXT NOT NULL,
                    Tipo TEXT NOT NULL,
                    Estado TEXT NOT NULL,
                    FechaEnvio TEXT NOT NULL
                );
            """);
        }
    }
}