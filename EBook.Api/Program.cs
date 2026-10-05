using EBook.Api.Middleware;
using EBook.Application.Extensions;
using EBook.Infrastructure.Data;
using EBook.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/eblog-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application and infrastructure services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionString ?? "Data Source=ebook.db");
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EBook API v1");
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAngularDev");

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and guest ClientId column exists (EnsureCreated won't alter existing DBs)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    EnsureFavoriteClientIdColumn(context);
}

try
{
    Log.Information("Starting EBook API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static void EnsureFavoriteClientIdColumn(AppDbContext context)
{
    var connection = context.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
        connection.Open();
    }

    using var check = connection.CreateCommand();
    check.CommandText = "SELECT COUNT(*) FROM pragma_table_info('Favorites') WHERE name = 'ClientId'";
    var exists = Convert.ToInt64(check.ExecuteScalar()) > 0;
    if (exists)
    {
        return;
    }

    using var alter = connection.CreateCommand();
    alter.CommandText = "ALTER TABLE Favorites ADD COLUMN ClientId TEXT NOT NULL DEFAULT ''";
    alter.ExecuteNonQuery();

    using var index = connection.CreateCommand();
    index.CommandText = "CREATE INDEX IF NOT EXISTS IX_Favorites_ClientId ON Favorites(ClientId)";
    index.ExecuteNonQuery();
}
