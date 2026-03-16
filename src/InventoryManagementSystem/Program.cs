// Program.cs — Full DI registration for all enterprise patterns
using FluentValidation;
using InventoryManagementSystem.Common.Behaviors;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Persistence;
using InventoryManagementSystem.Features.Batches.Repository;
using InventoryManagementSystem.Features.Categories.Repository;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Features.Products.Repository;
using InventoryManagementSystem.Features.PurchaseOrders.Repository;
using InventoryManagementSystem.Features.Suppliers.Repository;
using InventoryManagementSystem.Features.Warehouses.Repository;
using InventoryManagementSystem.Infrastructure.Middleware;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// ─── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

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
// DbConnectionFactory is Singleton: IConfiguration is resolved ONCE at startup,
// CreateConnection() produces a new MySqlConnection on every call (Factory Pattern).
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// ─── Unit of Work Pattern ─────────────────────────────────────────────────────
// Scoped: one UnitOfWork per HTTP request so Inventory command handlers share
// a single connection + transaction within a request.
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

// ─── MediatR + CQRS ───────────────────────────────────────────────────────────
// Registers all IRequestHandler<,> from this assembly automatically.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

// ─── Chain of Responsibility: MediatR Pipeline Behaviors ──────────────────────
// Order matters — registered behaviours wrap the handler in this sequence:
//   LoggingBehavior → ValidationBehavior → ExceptionHandlingBehavior → Handler
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

// ─── FluentValidation ─────────────────────────────────────────────────────────
// Scans the assembly for all AbstractValidator<T> implementations.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ─── Logging ──────────────────────────────────────────────────────────────────
builder.Services.AddLogging();

var app = builder.Build();

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
app.UseHttpsRedirection();
app.UseCors();
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
