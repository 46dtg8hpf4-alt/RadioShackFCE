using Microsoft.Data.Sqlite;
using Dapper;

namespace Orders.API.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db";
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS orders (
                id TEXT PRIMARY KEY,
                usuario_id TEXT NOT NULL,
                total REAL NOT NULL,
                estado TEXT NOT NULL,
                fecha_creacion TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS order_items (
                order_id TEXT NOT NULL,
                producto_id TEXT NOT NULL,
                cantidad INTEGER NOT NULL,
                precio_unitario REAL NOT NULL,
                FOREIGN KEY(order_id) REFERENCES orders(id)
            );
        """);
    }
}
