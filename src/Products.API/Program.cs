// Configuraciñon general de la aplicación

using Products.API.Services;
using Products.API.ExceptionHandlers;
using Microsoft.AspNetCore.Mvc;
using Products.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Obtener mensajes de error
            var validationErrors = context.ModelState.Values
                .SelectMany(value => value.Errors);

            string errors = "";

            foreach (var errorItem in validationErrors)
            {
                errors += errorItem.ErrorMessage + "; ";
            }

            // Crear respuesta estándar
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

builder.Services.AddSingleton<ProductService>(); //

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();