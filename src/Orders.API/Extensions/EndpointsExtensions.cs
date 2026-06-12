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
            })
            .WithTags("Orders")
            .WithSummary("Listar órdenes (filtro por usuarioId)")
            .Produces<IEnumerable<Order>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            //Segunda api para pedir orders por orderId
            app.MapGet("/api/orders/{id:guid}", async (Guid id, OrderRepository repo) =>
            {
                var orders = await repo.GetByIdAsync(id);

                if (orders == null)
                {
                    throw new NotFoundException("ORD-001", "La orden no fue encontrada");
                }

                return Results.Ok(orders);
            })
            .WithTags("Orders")
            .WithSummary("Obtener detalle de una orden")
            .Produces<Order>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // Endpoint para verificar si un producto tiene órdenes activas.
            // Será utilizado por Products API antes de permitir eliminar un producto (PRD-004).        
            app.MapGet(
                "/api/orders/product/{productId:guid}",
                async (Guid productId, OrderRepository repo) =>
            {
                bool hasOrders =
                    await repo.ProductHasActiveOrders(productId);

                return Results.Ok(hasOrders);
            })
            .WithTags("Orders")
            .WithSummary("Verifica si un producto tiene órdenes activas")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            //Tercer endpoint para crear una orden consultando Users.API y Products.API
            app.MapPost("/api/orders", async (CreateOrderRequest req, OrderRepository repo, Orders.API.Clients.UsersApiClient usersApi, Orders.API.Clients.ProductsApiClient productsApi) =>
            {
                if (req.Items == null || !req.Items.Any())
                {
                    throw new ValidationException("ORD-002", "Los datos de la orden son inválidos");
                }

                if (req.Items.Any(item => item.Cantidad <= 0))
                {
                    throw new ValidationException ("ORD-002", "Los datos de la orden son inválidos");
                }

                if (!await usersApi.UserExistsAsync(req.usuarioId))
                {
                    throw new NotFoundException("ORD-003", "Usuario no encontrado al crear la orden.");
                }

                var orderItems = new List<OrderItem>();
                decimal totalCalculado = 0;

                foreach (var item in req.Items)
                {
                    var product = await productsApi.GetProductAsync(item.ProductoId);
                    if (product == null)
                    {
                        throw new NotFoundException("ORD-004", "Producto no encontrado al crear la orden.");
                    }

                    if (item.Cantidad > product.Stock)
                    {
                        throw new BusinessRuleException("ORD-005", $"Stock insuficiente para '{product.Nombre}'. Disponible: {product.Stock}, solicitado: {item.Cantidad}.");
                    }

                    orderItems.Add(new OrderItem
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = product.Precio
                    });

                    totalCalculado += item.Cantidad * product.Precio;
                }

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
            })
            .WithTags("Orders")
            .WithSummary("Crear nueva orden")
            .Produces<Order>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            app.MapPut("/api/orders/{id:guid}/status", async (Guid id, UpdateOrderStatusRequest req, OrderRepository repo) =>
            {
                var ordenExistente = await repo.GetByIdAsync(id);

               
                if (ordenExistente == null)
                {
                    throw new NotFoundException("ORD-001", "La orden no fue encontrada");
                }

                if (ordenExistente.Estado == "Entregada" && req.Estado == "Pendiente")
                {
                    throw new NotFoundException("ORD-006", "Una orden en estado 'Entregada' no puede volver a 'Pendiente'.");
                }

                await repo.UpdateStatusAsync(id, req.Estado);

                var response = new UpdateOrderStatusResponse(
                    id,
                    req.Estado,
                    DateTime.UtcNow
                );

                return Results.Ok(response);

            })
            .WithTags("Orders")
            .WithSummary("Actualizar estado de la orden")
            .Produces<UpdateOrderStatusResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        }
    }
}
