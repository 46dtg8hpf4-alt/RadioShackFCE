using Cart.API.Models;
using Cart.API.Data;
using Cart.API.Exceptions;
using Cart.API.Clients;

namespace Cart.API.Extensions
{
    public static class EndpointsExtensions
    {
        public static void MapCartEndpoints(this WebApplication app)
        {
            // get /api/cart/{userId} - busco el carrito del chabon
            app.MapGet("/api/cart/{userId:guid}", async (Guid userId, CartRepository repo) =>
            {
                var cart = await repo.GetByUsuarioIdAsync(userId);

                if (cart == null)
                {
                    throw new NotFoundException("CRT-001", "Carrito no encontrado.");
                }

                return Results.Ok(cart);
            })
            .WithTags("Cart")
            .WithSummary("Obtener carrito del usuario")
            .Produces<Cart.API.Models.Cart>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // post /api/cart/{userId}/items - meto un producto al carrito
            app.MapPost("/api/cart/{userId:guid}/items", async (Guid userId, AddCartItemRequest req, CartRepository repo, ProductsApiClient productsApi) =>
            {
                // crt-004: me fijo q la cantidad no sea negativa o cero
                if (req.Cantidad <= 0)
                {
                    throw new ValidationException("CRT-004", "Cantidad inválida.");
                }

                // crt-002: pregunto a products si existe el coso
                var product = await productsApi.GetProductAsync(req.ProductoId);
                if (product == null)
                {
                    throw new NotFoundException("CRT-002", "Producto no encontrado.");
                }

                // crt-003: checkeo q alcance el stock
                // sumo lo q ya tiene en el carrito mas lo q quiere agregar ahora
                int cantidadEnCarrito = 0;
                var cartExistente = await repo.GetByUsuarioIdAsync(userId);
                if (cartExistente != null)
                {
                    foreach (var item in cartExistente.Items)
                    {
                        if (item.ProductoId == req.ProductoId)
                        {
                            cantidadEnCarrito = item.Cantidad;
                            break;
                        }
                    }
                }

                int cantidadTotal = cantidadEnCarrito + req.Cantidad;
                if (cantidadTotal > product.Stock)
                {
                    throw new BusinessRuleException("CRT-003",
                        $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {cantidadTotal}.");
                }

                var cart = await repo.AddOrUpdateItemAsync(userId, req.ProductoId, req.Cantidad);
                return Results.Ok(cart);
            })
            .WithTags("Cart")
            .WithSummary("Agregar producto al carrito")
            .Produces<Cart.API.Models.Cart>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // put /api/cart/{userId}/items/{productId} - le cambio la cantidad a un item
            app.MapPut("/api/cart/{userId:guid}/items/{productId:guid}", async (Guid userId, Guid productId, UpdateCartItemRequest req, CartRepository repo, ProductsApiClient productsApi) =>
            {
                // CRT-004: validar cantidad
                if (req.Cantidad <= 0)
                {
                    throw new ValidationException("CRT-004", "Cantidad inválida.");
                }

                // crt-001: me aseguro q el carrito exista posta
                var cartExistente = await repo.GetByUsuarioIdAsync(userId);
                if (cartExistente == null)
                {
                    throw new NotFoundException("CRT-001", "Carrito no encontrado.");
                }

                // me fijo q el item este en este carrito
                bool itemEncontrado = false;
                foreach (var item in cartExistente.Items)
                {
                    if (item.ProductoId == productId)
                    {
                        itemEncontrado = true;
                        break;
                    }
                }

                if (!itemEncontrado)
                {
                    throw new NotFoundException("CRT-002", "Producto no encontrado.");
                }

                // CRT-003: validar stock
                var product = await productsApi.GetProductAsync(productId);
                if (product == null)
                {
                    throw new NotFoundException("CRT-002", "Producto no encontrado.");
                }

                if (req.Cantidad > product.Stock)
                {
                    throw new BusinessRuleException("CRT-003",
                        $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {req.Cantidad}.");
                }

                var cart = await repo.UpdateItemCantidadAsync(userId, productId, req.Cantidad);
                return Results.Ok(cart);
            })
            .WithTags("Cart")
            .WithSummary("Actualizar cantidad de un item")
            .Produces<Cart.API.Models.Cart>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // delete /api/cart/{userId}/items/{productId} - saco el producto del carrito
            app.MapDelete("/api/cart/{userId:guid}/items/{productId:guid}", async (Guid userId, Guid productId, CartRepository repo) =>
            {
                // CRT-001: validar que exista el carrito
                var cartExistente = await repo.GetByUsuarioIdAsync(userId);
                if (cartExistente == null)
                {
                    throw new NotFoundException("CRT-001", "Carrito no encontrado.");
                }

                var removido = await repo.RemoveItemAsync(userId, productId);
                if (!removido)
                {
                    throw new NotFoundException("CRT-002", "Producto no encontrado.");
                }

                return Results.NoContent();
            })
            .WithTags("Cart")
            .WithSummary("Quitar un producto del carrito")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

            // delete /api/cart/{userId} - borro todo el carrito
            app.MapDelete("/api/cart/{userId:guid}", async (Guid userId, CartRepository repo) =>
            {
                // CRT-001: validar que exista el carrito
                var existe = await repo.CartExistsAsync(userId);
                if (!existe)
                {
                    throw new NotFoundException("CRT-001", "Carrito no encontrado.");
                }

                await repo.ClearCartAsync(userId);
                return Results.NoContent();
            })
            .WithTags("Cart")
            .WithSummary("Vaciar carrito completo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);
        }
    }
}
