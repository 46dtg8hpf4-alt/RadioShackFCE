using Users.API;
using Users.API.Services;

using Users.API.Extensions;
using Serilog;
using Serilog.Events;

using System.Reflection;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            var errorMessageDetallado = string.Join("; ", errors);

            var correlationId = context.HttpContext.Items["CorrelationId"]?.ToString();

            var errorResponse = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "Bad Request",
                status = StatusCodes.Status400BadRequest,
                detail = "Los datos provistos no pasaron las validaciones del sistema.",
                instance = context.HttpContext.Request.Path.Value,
                errorCode = "USR-002",
                errorMessage = errorMessageDetallado,
                correlationId = correlationId
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(errorResponse)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });
builder.Services.AddExceptionHandler<Users.API.ExceptionHandlers.BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<Users.API.ExceptionHandlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddAppHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Users.API.Middlewares.CorrelationIdDelegatingHandler>();



var app = builder.Build();

app.UseMiddleware<Users.API.Middlewares.CorrelationIdMiddleware>();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>()
        .Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, ex) =>
        (ex != null) ? LogEventLevel.Error :
        (httpContext.Request.Path.StartsWithSegments("/health"))
            ? LogEventLevel.Verbose : LogEventLevel.Information;
});

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.MapControllers();
app.MapAppHealthChecks();

app.Run();
