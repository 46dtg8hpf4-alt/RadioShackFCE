using Microsoft.Data.Sqlite;
using Dapper;
using Cart.API.Models;

namespace Cart.API.Data;

public class CartRepository
{
    private readonly string _connectionString;

    public CartRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";
        EnsureCreated();
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    private void EnsureCreated()
    {
        using var conn = CreateConnection();
        conn.Open();

        var sql = """
            CREATE TABLE IF NOT EXISTS carts (
                usuario_id TEXT PRIMARY KEY,
                fecha_actualizacion TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS cart_items (
                usuario_id TEXT NOT NULL,
                producto_id TEXT NOT NULL,
                cantidad INTEGER NOT NULL,
                PRIMARY KEY (usuario_id, producto_id),
                FOREIGN KEY (usuario_id) REFERENCES carts(usuario_id)
            );
        """;

        conn.Execute(sql);
    }

    public async Task<Cart.API.Models.Cart?> GetByUsuarioIdAsync(Guid usuarioId)
    {
        using var conn = CreateConnection();

        var cartSql = """
            SELECT usuario_id AS UsuarioId, fecha_actualizacion AS FechaActualizacion
            FROM carts
            WHERE usuario_id = @UsuarioId
        """;

        var cartRow = await conn.QueryFirstOrDefaultAsync<CartRow>(cartSql, new { UsuarioId = usuarioId.ToString() });

        if (cartRow == null) return null;

        var itemsSql = """
            SELECT producto_id AS ProductoId, cantidad AS Cantidad
            FROM cart_items
            WHERE usuario_id = @UsuarioId
        """;

        var itemRows = await conn.QueryAsync<CartItemRow>(itemsSql, new { UsuarioId = usuarioId.ToString() });

        var items = new List<CartItem>();
        foreach (var row in itemRows)
        {
            items.Add(new CartItem
            {
                ProductoId = Guid.Parse(row.ProductoId),
                Cantidad = row.Cantidad
            });
        }

        return new Cart.API.Models.Cart
        {
            UsuarioId = Guid.Parse(cartRow.UsuarioId),
            Items = items.ToArray(),
            FechaActualizacion = cartRow.FechaActualizacion
        };
    }

    public async Task<Cart.API.Models.Cart> AddOrUpdateItemAsync(Guid usuarioId, Guid productoId, int cantidad)
    {
        using var conn = CreateConnection();
        conn.Open();

        // meto o actualizo el carrito (si ya existe, solo le cambio la fecha)
        var upsertCartSql = """
            INSERT INTO carts (usuario_id, fecha_actualizacion)
            VALUES (@UsuarioId, @Fecha)
            ON CONFLICT(usuario_id) DO UPDATE SET fecha_actualizacion = @Fecha
        """;

        var ahora = DateTime.UtcNow;
        await conn.ExecuteAsync(upsertCartSql, new
        {
            UsuarioId = usuarioId.ToString(),
            Fecha = ahora.ToString("o")
        });

        // me fijo si el producto ya esta en el carrito
        var existeSql = """
            SELECT cantidad FROM cart_items
            WHERE usuario_id = @UsuarioId AND producto_id = @ProductoId
        """;

        var cantidadExistente = await conn.QueryFirstOrDefaultAsync<int?>(existeSql, new
        {
            UsuarioId = usuarioId.ToString(),
            ProductoId = productoId.ToString()
        });

        if (cantidadExistente != null)
        {
            // si ya estaba, le sumo la cant nueva
            var updateSql = """
                UPDATE cart_items SET cantidad = cantidad + @Cantidad
                WHERE usuario_id = @UsuarioId AND producto_id = @ProductoId
            """;

            await conn.ExecuteAsync(updateSql, new
            {
                Cantidad = cantidad,
                UsuarioId = usuarioId.ToString(),
                ProductoId = productoId.ToString()
            });
        }
        else
        {
            // sino lo inserto como item nuevo
            var insertSql = """
                INSERT INTO cart_items (usuario_id, producto_id, cantidad)
                VALUES (@UsuarioId, @ProductoId, @Cantidad)
            """;

            await conn.ExecuteAsync(insertSql, new
            {
                UsuarioId = usuarioId.ToString(),
                ProductoId = productoId.ToString(),
                Cantidad = cantidad
            });
        }

        conn.Close();
        return await GetByUsuarioIdAsync(usuarioId) ?? throw new Exception("Error al recuperar el carrito después de agregar item.");
    }

    public async Task<Cart.API.Models.Cart> UpdateItemCantidadAsync(Guid usuarioId, Guid productoId, int nuevaCantidad)
    {
        using var conn = CreateConnection();
        conn.Open();

        // aca piso la cantidad q tenia con la nueva q me pasan
        var updateItemSql = """
            UPDATE cart_items SET cantidad = @Cantidad
            WHERE usuario_id = @UsuarioId AND producto_id = @ProductoId
        """;

        var filas = await conn.ExecuteAsync(updateItemSql, new
        {
            Cantidad = nuevaCantidad,
            UsuarioId = usuarioId.ToString(),
            ProductoId = productoId.ToString()
        });

        if (filas == 0) return null!;

        // le actualizo la fecha tmb
        var updateCartSql = """
            UPDATE carts SET fecha_actualizacion = @Fecha
            WHERE usuario_id = @UsuarioId
        """;

        await conn.ExecuteAsync(updateCartSql, new
        {
            Fecha = DateTime.UtcNow.ToString("o"),
            UsuarioId = usuarioId.ToString()
        });

        conn.Close();
        return await GetByUsuarioIdAsync(usuarioId) ?? throw new Exception("Error al recuperar el carrito después de actualizar item.");
    }

    public async Task<bool> RemoveItemAsync(Guid usuarioId, Guid productoId)
    {
        using var conn = CreateConnection();

        var sql = """
            DELETE FROM cart_items
            WHERE usuario_id = @UsuarioId AND producto_id = @ProductoId
        """;

        var filas = await conn.ExecuteAsync(sql, new
        {
            UsuarioId = usuarioId.ToString(),
            ProductoId = productoId.ToString()
        });

        if (filas > 0)
        {
            var updateSql = """
                UPDATE carts SET fecha_actualizacion = @Fecha
                WHERE usuario_id = @UsuarioId
            """;

            await conn.ExecuteAsync(updateSql, new
            {
                Fecha = DateTime.UtcNow.ToString("o"),
                UsuarioId = usuarioId.ToString()
            });
        }

        return filas > 0;
    }

    public async Task<bool> ClearCartAsync(Guid usuarioId)
    {
        using var conn = CreateConnection();

        var deleteItemsSql = """
            DELETE FROM cart_items WHERE usuario_id = @UsuarioId
        """;
        await conn.ExecuteAsync(deleteItemsSql, new { UsuarioId = usuarioId.ToString() });

        var deleteCartSql = """
            DELETE FROM carts WHERE usuario_id = @UsuarioId
        """;
        var filas = await conn.ExecuteAsync(deleteCartSql, new { UsuarioId = usuarioId.ToString() });

        return filas > 0;
    }

    public async Task<bool> CartExistsAsync(Guid usuarioId)
    {
        using var conn = CreateConnection();

        var sql = "SELECT COUNT(1) FROM carts WHERE usuario_id = @UsuarioId";
        var count = await conn.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId.ToString() });

        return count > 0;
    }

    public async Task<bool> ItemExistsInCartAsync(Guid usuarioId, Guid productoId)
    {
        using var conn = CreateConnection();

        var sql = """
            SELECT COUNT(1) FROM cart_items
            WHERE usuario_id = @UsuarioId AND producto_id = @ProductoId
        """;

        var count = await conn.ExecuteScalarAsync<int>(sql, new
        {
            UsuarioId = usuarioId.ToString(),
            ProductoId = productoId.ToString()
        });

        return count > 0;
    }

    // clases de dapper no renegar con los mapeos
    private class CartRow
    {
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaActualizacion { get; set; }
    }

    private class CartItemRow
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
