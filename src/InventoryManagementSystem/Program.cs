using System.Text;
using FluentValidation;
using InventoryManagementSystem.Common.Behaviors;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Persistence;
using InventoryManagementSystem.Data.Seeds;
using InventoryManagementSystem.Features.Auth.Repository;
using InventoryManagementSystem.Features.Auth.Services;
using InventoryManagementSystem.Features.Batches.Repository;
using InventoryManagementSystem.Features.Categories.Repository;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Features.Products.Repository;
using InventoryManagementSystem.Features.PurchaseOrders.Repository;
using InventoryManagementSystem.Features.Suppliers.Repository;
using InventoryManagementSystem.Features.Warehouses.Repository;
using InventoryManagementSystem.Infrastructure.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ─── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Accept camelCase from the React frontend and respond in camelCase
        opts.JsonSerializerOptions.PropertyNamingPolicy        = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        opts.JsonSerializerOptions.NumberHandling              = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });


// ─── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "📦 Inventory Management System API",
        Version     = "v1",
        Description = "Enterprise .NET 8 Web API — CQRS · MediatR · DDD · Repository · UoW · Factory · Singleton · Chain of Responsibility"
    });
    c.EnableAnnotations();
});

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ─── Factory + Singleton Pattern ──────────────────────────────────────────────
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// ─── Unit of Work Pattern ─────────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ─── Repository Pattern (feature repositories) ────────────────────────────────
builder.Services.AddScoped<IProductRepository,      ProductRepository>();
builder.Services.AddScoped<ISupplierRepository,     SupplierRepository>();
builder.Services.AddScoped<IWarehouseRepository,    WarehouseRepository>();
builder.Services.AddScoped<ICategoryRepository,     CategoryRepository>();
builder.Services.AddScoped<IBatchRepository,        BatchRepository>();
builder.Services.AddScoped<IStockLevelRepository,   StockLevelRepository>();
builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository,    PurchaseOrderRepository>();

// ─── Distributed Caching (Redis) ──────────────────────────────────────────────
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName  = "IMS_";
});
builder.Services.AddSingleton<InventoryManagementSystem.Common.Interfaces.ICacheService, InventoryManagementSystem.Common.Services.RedisCacheService>();


// ─── Auth Feature ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IJwtService,  JwtService>();

// ─── JWT Authentication ───────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing from appsettings.json.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// ─── Data Seeder (Deliverable #8) ─────────────────────────────────────────────
builder.Services.AddScoped<InitialDataSeeder>();

// ─── MediatR + CQRS ───────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

// ─── Chain of Responsibility: MediatR Pipeline Behaviors ──────────────────────
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

// ─── FluentValidation ─────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ─── Logging ──────────────────────────────────────────────────────────────────
builder.Services.AddLogging();

var app = builder.Build();

// ─── Auto-migrate: ensure AppUser table exists ────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var migration = new InventoryManagementSystem.Data.Seeds.AppUserTableMigration(
        scope.ServiceProvider.GetRequiredService<InventoryManagementSystem.Common.Interfaces.IDbConnectionFactory>(),
        scope.ServiceProvider.GetRequiredService<ILogger<InventoryManagementSystem.Data.Seeds.AppUserTableMigration>>());
    await migration.RunAsync();
}

// NOTE: Auto-seed removed — database already contains live data.

// ─── Global Exception Middleware (first in pipeline) ──────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

// ─── Swagger ──────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Management System API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "IMS Enterprise API";
});

// ─── Pipeline ─────────────────────────────────────────────────────────────────
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.UseMiddleware<JwtBlacklistMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ─── Health Endpoint ──────────────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new
{
    status    = "Healthy",
    timestamp = DateTime.UtcNow,
    patterns  = new[]
    {
        "CQRS via MediatR",
        "Repository Pattern",
        "Unit of Work",
        "Factory Pattern (DbConnectionFactory)",
        "Singleton Pattern (DbConnectionFactory registered as Singleton)",
        "Chain of Responsibility (MediatR Pipeline Behaviors)",
        "Domain Driven Design (Domain/Entities, Domain/ValueObjects, Domain/Enumerations)",
        "Feature-based Folder Structure"
    }
}));

app.Run();
