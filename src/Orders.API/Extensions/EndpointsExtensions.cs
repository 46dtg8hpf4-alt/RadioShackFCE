using Orders.API.Models;
using Orders.API.Data;
using Orders.API.Exceptions;

namespace Orders.API.Extensions
{
    public static class EndpointsExtensions
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {

            //Primera api para pedir orders por usuarioId
            app.MapGet("/api/orders", async (Guid? usuarioId, OrderRepository repo) =>
            {
                var orders = await repo.GetAllAsync(usuarioId);
                return Results.Ok(orders);
            });

            //Segunda api para pedir orders por orderId
            app.MapGet("/api/orders/{id:guid}", async (Guid id, OrderRepository repo) =>
            {
                var orders = await repo.GetByIdAsync(id);

                if (orders == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(orders);
            });

            //Tercer endpoint para crear una orden con cantidad y calcula total(fijando un precio para su prueba).
            app.MapPost("/api/orders", async (CreateOrderRequest req, OrderRepository repo) =>
            {
                if (req.Items == null || !req.Items.Any())
                {
                    throw new BusinessRuleException("ORD-002", "Los datos de la orden son inválidos");
                }

                if (req.Items.Any(item => item.Cantidad <= 0))
                {
                    throw new BusinessRuleException("ORD-002", "Los datos de la orden son inválidos");
                }

                decimal precioSimulado = 1500.00m; //Esto es para probarlo en swagger y que me de un total

                var orderItems = req.Items.Select(i => new OrderItem
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = precioSimulado,
                }).ToList();

                decimal totalCalculado = orderItems.Sum(i => i.Cantidad * i.PrecioUnitario);

                var nuevaOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    usuarioId = req.usuarioId,
                    Items = orderItems,
                    Total = totalCalculado,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now,

                };

                await repo.CreateAsync(nuevaOrder);

                return Results.Created($"/api/orders/{nuevaOrder.Id}", nuevaOrder);
            });

            app.MapPut("/api/orders/{id:guid}/status", async (Guid id, UpdateOrderStatusRequest req, OrderRepository repo) =>
            {
                var ordenExistente = await repo.GetByIdAsync(id);

                // esto lo tengo que cambiar para cuando este lo de exception handlers.
                if (ordenExistente == null)
                {
                    return Results.NotFound(new { message = "Orden no encontrada" });
                }

                if (ordenExistente.Estado == "Entregada" && req.Estado == "Pendiente")
                {
                    return Results.Conflict(new { message = "Una orden en estado 'Entregada' no puede volver a 'Pendiente'" });
                }

                await repo.UpdateStatusAsync(id, req.Estado);

                var response = new UpdateOrderStatusResponse(
                    id,
                    req.Estado,
                    DateTime.UtcNow
                );

                return Results.Ok(response);

            });

        }
    }
}
