// ============================================================
// FILE: src/InventoryManagement.API/Program.cs
// ============================================================
using System.Text;
using InventoryManagement.API.Authorization;
using InventoryManagement.API.Configurations;
using InventoryManagement.API.Middleware;
using InventoryManagement.Application;
using InventoryManagement.Infrastructure;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── 1. App Configuration & Logging ──────────────────────────
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// (In production, wire up Serilog here as configured in appsettings.json)

// ── 2. Add Layer Dependencies ───────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── 3. Add Authentication & Authorization ───────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
string secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in prod
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    // Roles mapping to policies
    options.AddPolicy(Constants.Policies.AdminOnly, policy => policy.RequireRole(Constants.Roles.Admin));
    options.AddPolicy(Constants.Policies.CanManageProducts, policy => policy.RequireRole(Constants.Roles.Admin, Constants.Roles.Manager));
    options.AddPolicy(Constants.Policies.CanApproveOrders, policy => policy.RequireRole(Constants.Roles.Admin, Constants.Roles.Manager));
    options.AddPolicy(Constants.Policies.CanViewReports, policy => policy.RequireAuthenticatedUser());

    // Custom requirements
    options.AddPolicy("RequiresExperience", policy => policy.Requirements.Add(new MinimumExperienceRequirement("Senior")));
    options.AddPolicy("WarehouseSpecific", policy => policy.Requirements.Add(new WarehouseAccessRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, MinimumExperienceHandler>();
builder.Services.AddScoped<IAuthorizationHandler, WarehouseAccessHandler>();

// ── 4. Add Web API Services ─────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddHttpContextAccessor();

// CORS Configuration
var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(Constants.CorsPolicies.AllowSpecificOrigins, policy =>
    {
        policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

// ── 5. Configure HTTP Request Pipeline ──────────────────────

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(Constants.CorsPolicies.AllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Used for Integration Tests
public partial class Program { }
