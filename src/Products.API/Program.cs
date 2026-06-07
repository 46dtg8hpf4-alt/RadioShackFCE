// Configuración general de la aplicación

using Products.API.Services;
using Products.API.ExceptionHandlers;
using Microsoft.AspNetCore.Mvc;
using Products.API.Models;
using Products.API.Data;
using Products.API.Extensions;
using Serilog;
using Products.API.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppLogging();

// Configuración de Controllers y validaciones
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Acumular mensajes de error de validación
            string errors = "";

            foreach (var modelState in context.ModelState.Values)
            {
                foreach (var errorItem in modelState.Errors)
                {
                    errors += errorItem.ErrorMessage + "; ";
                }
            }

            // Crear respuesta estándar de error
            ApiError error = new ApiError();

            error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            error.Title = "Bad Request";
            error.Status = 400;
            error.Detail = "Los datos enviados son inválidos.";
            error.Instance = context.HttpContext.Request.Path;
            error.ErrorCode = "PRD-002";
            error.ErrorMessage = errors;

            return new BadRequestObjectResult(error);
        };
    });

// Registrar ProductService como Singleton
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Checks
builder.Services.AddHealthChecks();

// Exception Handlers
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Problem Details para errores HTTP
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>()
        .Initialize();
}

// Swagger solamente en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de manejo global de excepciones
app.UseExceptionHandler();

app.UseAuthorization();

// Endpoints de Health Checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// Endpoints de Controllers
app.MapControllers();

app.Run();