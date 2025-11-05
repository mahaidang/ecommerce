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
        // ✅ EF Core
        services.AddDbContext<InventoryDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        // Fix for CS0266 and CS1662:
        // Use an explicit cast to IInventoryDbContext when resolving InventoryDbContext.
        services.AddScoped<IInventoryDbContext>(sp => (IInventoryDbContext)sp.GetRequiredService<InventoryDbContext>());

        // ✅ RabbitMQ
        services.AddMassTransit(x =>
        {
            // Đăng ký các consumer
            x.AddConsumer<InventoryReserveConsumer>();
            x.AddConsumer<InventoryReleaseConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config["RabbitMq:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(config["RabbitMq:User"] ?? "guest");
                    h.Password(config["RabbitMq:Pass"] ?? "guest");
                });

                // Tự động tạo exchange/queue từ các consumer
                cfg.ConfigureEndpoints(context);
            });
        });

        //services.AddSingleton<IConnection>(sp =>
        //    sp.GetRequiredService<IConnectionFactory>().CreateConnection());

        //// ✅ Background Worker (Saga)
        //services.AddHostedService<InventorySagaConsumer>();

        // ✅ gRPC
        services.AddGrpc();

        return services;
    }
}
