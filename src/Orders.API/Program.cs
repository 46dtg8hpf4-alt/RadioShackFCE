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
        builder.Host.UseSerilog();

        //Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //base de datos

        builder.Services.AddScoped<OrderRepository>();
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
        builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();  
        builder.Services.AddProblemDetails();

        var app = builder.Build();

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


