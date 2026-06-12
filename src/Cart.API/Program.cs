using Cart.API.Data;
using Cart.API.ExceptionHandlers;
using Cart.API.Extensions;
using Serilog;

namespace Cart.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // configuro serilog
        builder.Host.AddAppLogging();

        // inyecto el repo de la db
        builder.Services.AddScoped<CartRepository>();
        builder.Services.AddTransient<DatabaseInitializer>();

        // meto health checks y swagger q saque del pdf
        builder.Services.AddAppServices();

        // clientes http para llamar a las otras apis
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<Cart.API.Middleware.CorrelationIdHandler>();

        builder.Services.AddHttpClient<Cart.API.Clients.ProductsApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5151");
        }).AddHttpMessageHandler<Cart.API.Middleware.CorrelationIdHandler>();

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

        // meto los middlewares q piden en el tp
        app.UseMiddleware<Cart.API.Middleware.CorrelationIdMiddleware>();
        app.UseAppMiddleware();

        // esto es clave para q agarre los exception handlers nuestros
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapCartEndpoints();

        app.Run();
    }
}
