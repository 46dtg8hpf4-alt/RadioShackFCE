using Microsoft.Data.Sqlite;
using Dapper;
using Orders.API.Models;

namespace Orders.API.Data;

public class OrderRepository
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public OrderRepository(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db";
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Order>> GetAllAsync(Guid? usuarioId = null)
    {
        using var conn = CreateConnection();

        var sql = """
            SELECT id AS Id, usuario_id AS UsuarioId, total AS Total, 
                   estado AS Estado, fecha_creacion AS FechaCreacion 
            FROM orders 
            WHERE (@UsuarioId IS NULL OR usuario_id = @UsuarioId)
            ORDER BY fecha_creacion DESC
        """;

        var orderRows = await conn.QueryAsync<OrderRow>(sql, new { UsuarioId = usuarioId?.ToString() });

        var orders = new List<Order>();

        if (orderRows == null || !orderRows.Any())
        {
            return orders;
        }

        foreach (var row in orderRows)
        {
            var nuevaOrder = new Order
            {
                Id = Guid.Parse(row.Id),
                usuarioId = Guid.Parse(row.UsuarioId),
                Total = row.Total,
                Estado = row.Estado,
                FechaCreacion = row.FechaCreacion
            };

            orders.Add(nuevaOrder);
        }

        var itemsSql = """
        SELECT order_id AS OrderId, producto_id AS ProductoId, 
        cantidad AS Cantidad, precio_unitario AS PrecioUnitario 
        FROM order_items
        """;

        var allItems = await conn.QueryAsync<OrderItemRow>(itemsSql);

        foreach (var order in orders)
        {
            foreach (var itemRow in allItems)
            {
                if (itemRow.OrderId == order.Id.ToString())
                {
                    var nuevoItem = new OrderItem
                    {
                        ProductoId = Guid.Parse(itemRow.ProductoId),
                        Cantidad = itemRow.Cantidad,
                        PrecioUnitario = itemRow.PrecioUnitario
                    };

                    order.Items.Add(nuevoItem);

                }
            }
        }
        return orders;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        using var conn = CreateConnection();

        var orderSql = """
            SELECT id AS Id, usuario_id AS UsuarioId, total AS Total, 
                   estado AS Estado, fecha_creacion AS FechaCreacion 
            FROM orders 
            WHERE id = @Id
        """;

        var row = await conn.QueryFirstOrDefaultAsync<OrderRow>(orderSql, new { Id = id.ToString() });

        if (row == null) return null;

        var order = new Order
        {
            Id = Guid.Parse(row.Id),
            usuarioId = Guid.Parse(row.UsuarioId),
            Total = row.Total,
            Estado = row.Estado,
            FechaCreacion = row.FechaCreacion
        };

        var itemsSql = """
            SELECT producto_id AS ProductoId, cantidad AS Cantidad, precio_unitario AS PrecioUnitario 
            FROM order_items 
            WHERE order_id = @OrderId
        """;

        var itemsRows = await conn.QueryAsync<OrderItemRow>(itemsSql, new { OrderId = id.ToString() });

        foreach (var itemRow in itemsRows)
        {
            var nuevoItem = new OrderItem
            {
                ProductoId = Guid.Parse(itemRow.ProductoId),
                Cantidad = itemRow.Cantidad,
                PrecioUnitario = itemRow.PrecioUnitario
            };

            order.Items.Add(nuevoItem);
        }

        return order;
    }
        
    public async Task<Order> CreateAsync(Order order)
    {
        using var conn = CreateConnection();

        var orderSql = """
            INSERT INTO orders (id, usuario_id, total, estado, fecha_creacion)
            VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion);
        """;
        await conn.ExecuteAsync(orderSql, new
        {
            Id = order.Id.ToString(),
            UsuarioId = order.usuarioId.ToString(),
            order.Total,
            order.Estado,
            //aca el ToString lleva "o" porque sino guarda la fecha de mi pc
            FechaCreacion = order.FechaCreacion.ToString("o")
        });

        var itemSql = """
            INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario)
            VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario);
        """;

        foreach (var item in order.Items)
        {
            await conn.ExecuteAsync(itemSql, new
            {
                OrderId = order.Id.ToString(),
                ProductoId = item.ProductoId.ToString(),
                item.Cantidad,
                item.PrecioUnitario
            });
        }

        return order;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string nuevoEstado)
    {
        using var conn = CreateConnection();

        var sql = """
            UPDATE orders
            SET estado = @Estado
            WHERE id = @Id
        """;
        var filasAfectadas = await conn.ExecuteAsync(sql, new
        {
            Estado = nuevoEstado,
            Id = id.ToString()
        });

        return filasAfectadas > 0;
    }

    private class OrderRow
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }

    private class OrderItemRow
    {
        public string OrderId { get; set; } = string.Empty;
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    // Verifica si existe alguna orden pendiente que contenga el producto indicado.
        
        public async Task<bool> ProductHasActiveOrders(Guid productId)
    {
        var orders = await GetAllAsync();

        foreach (var order in orders)
        {
            if (order.Estado == "Pendiente")
            {
                foreach (var item in order.Items)
                {
                    if (item.ProductoId == productId)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

