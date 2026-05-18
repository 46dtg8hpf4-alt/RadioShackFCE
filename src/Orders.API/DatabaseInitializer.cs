using Microsoft.Data.Sqlite;
using Dapper;

namespace Orders.API;

public class DatabaseInitializer
{
    private readonly IConfiguration _config;

    public DatabaseInitializer(IConfiguration config)
    {
        _config = config;
    }

    public void Initialize()
    {
        var connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Codigo copiado del documento
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS orders (
                id TEXT PRIMARY KEY,
                usuario_id TEXT NOT NULL,
                total REAL NOT NULL DEFAULT 0,
                estado TEXT NOT NULL,
                fecha_creacion TEXT NOT NULL
            );
        """);

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS order_items (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id TEXT NOT NULL,
                producto_id TEXT NOT NULL,
                cantidad INTEGER NOT NULL,
                precio_unitario REAL NOT NULL,
                FOREIGN KEY(order_id) REFERENCES orders(id) ON DELETE CASCADE
            );
        """);
    }
}