var builder = WebApplication.CreateBuilder(args);

// Register controller services for dependency injection.
builder.Services.AddControllers();

// Configure CORS to allow the React frontend origin.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:7200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register the built-in ASP.NET Core health checks.
builder.Services.AddHealthChecks();

var app = builder.Build();

// Enable CORS middleware — must be called before MapControllers.
app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Map the built-in health check endpoint at /healthz (ASP.NET convention).
app.MapHealthChecks("/healthz");

app.Run();
