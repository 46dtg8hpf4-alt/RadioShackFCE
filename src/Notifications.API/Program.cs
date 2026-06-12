using Notifications.API.Clients;
using Notifications.API.Data;
using Notifications.API.ExceptionHandlers;
using Notifications.API.Extensions;
using Notifications.API.Middleware;
using Notifications.API.Services;
using Serilog;

// app.UseSerilogRequestLogging();

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging();

builder.Services.AddControllers();

builder.Services.AddSingleton<NotificationRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddScoped<NotificationService>();

builder.Services.AddHttpClient<UsersApiClient>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.MapControllers();

app.Run();