using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Consumers;
using Inventory.Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // ✅ EF Core (DbContext)
        services.AddDbContext<InventoryDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        // ✅ Interface mapping
        services.AddScoped<IInventoryDbContext>(sp =>
            sp.GetRequiredService<InventoryDbContext>());

        // ✅ MassTransit (RabbitMQ)
        services.AddMassTransit(x =>
        {
            // Register consumers (các consumer xử lý event từ Orchestrator)
            x.AddConsumer<InventoryReserveConsumer>();
            x.AddConsumer<InventoryReleaseConsumer>();
            x.AddConsumer<CheckStockConsumer>();
            x.AddConsumer<CommitStockConsumer>();


            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config["RabbitMq:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(config["RabbitMq:User"] ?? "guest");
                    h.Password(config["RabbitMq:Pass"] ?? "guest");
                });

                // Auto-configure queue/exchange từ consumer
                cfg.ConfigureEndpoints(context);
            });
        });

        // ✅ gRPC (nếu có)
        services.AddGrpc();

        return services;
    }
}
