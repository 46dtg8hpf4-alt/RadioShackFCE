using Users.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            var errorMessageDetallado = string.Join("; ", errors);

            var errorResponse = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "Bad Request",
                status = StatusCodes.Status400BadRequest,
                detail = "Los datos provistos no pasaron las validaciones del sistema.",
                instance = context.HttpContext.Request.Path.Value,
                errorCode = "USR-002",
                errorMessage = errorMessageDetallado
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(errorResponse)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });
builder.Services.AddExceptionHandler<Users.API.ExceptionHandlers.BusinessRuleExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
