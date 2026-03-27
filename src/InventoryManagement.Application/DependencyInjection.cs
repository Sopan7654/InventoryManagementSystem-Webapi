// ============================================================
// FILE: src/InventoryManagement.Application/DependencyInjection.cs
// ============================================================
using FluentValidation;
using InventoryManagement.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Application
{
    /// <summary>
    /// Registers Application layer services into the DI container.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            // MediatR — registers all handlers in this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // AutoMapper — registers all profiles in this assembly
            services.AddAutoMapper(assembly);

            // FluentValidation — registers all validators in this assembly
            services.AddValidatorsFromAssembly(assembly);

            // Pipeline Behaviors (order matters: Logging → Validation → Caching → Transaction)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            return services;
        }
    }
}
