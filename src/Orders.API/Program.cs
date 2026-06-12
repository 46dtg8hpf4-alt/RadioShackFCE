using Orders.API.Data;
using Orders.API.ExceptionHandlers;
using Orders.API.Extensions;
using Serilog;

namespace Orders.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Serilog
        builder.Host.AddAppLogging();

        //base de datos
        builder.Services.AddScoped<OrderRepository>();
        builder.Services.AddTransient<DatabaseInitializer>();
        
        // Servicios consolidados (Health Checks y Swagger) desde el PDF Componentes_MiniApi
        builder.Services.AddAppServices();

        // HttpClients for cross-service communication
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<Orders.API.Middleware.CorrelationIdHandler>();

        builder.Services.AddHttpClient<Orders.API.Clients.UsersApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5166");
        }).AddHttpMessageHandler<Orders.API.Middleware.CorrelationIdHandler>();

        builder.Services.AddHttpClient<Orders.API.Clients.ProductsApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5151");
        }).AddHttpMessageHandler<Orders.API.Middleware.CorrelationIdHandler>();

        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
        builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();  
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().Initialize();
        }

        // Correlation ID and Serilog Request Logging, and Health Checks map endpoints
        app.UseMiddleware<Orders.API.Middleware.CorrelationIdMiddleware>();
        app.UseAppMiddleware();

        //para que me tire bien los exception handlers
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapOrderEndpoints();

        app.Run();

    }
}


