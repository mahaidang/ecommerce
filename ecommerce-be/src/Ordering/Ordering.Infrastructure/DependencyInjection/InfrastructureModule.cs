using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common;
using Ordering.Application.Inventory;
using Ordering.Infrastructure.Inventory;
using Ordering.Infrastructure.Outbox;
using Ordering.Infrastructure.Saga;
using OrderingService.Infrastructure.Models;
using RabbitMQ.Client;


namespace Ordering.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // ✅ DbContext Factory (không dùng AddDbContext trực tiếp)
        services.AddDbContextFactory<OrderingDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<OrderingDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<OrderingDbContext>>().CreateDbContext());

        services.AddScoped<IOrderingDbContext>(sp =>
            sp.GetRequiredService<OrderingDbContext>());

        // ✅ gRPC Client (tới InventoryService)
        services.AddSingleton(sp =>
        {
            var addr = config["Grpc:InventoryBaseUrl"]!;
            var ch = GrpcChannel.ForAddress(addr);
            return new InventoryService.Grpc.Inventory.InventoryClient(ch);
        });
        services.AddSingleton<IInventoryStockClient, GrpcInventoryStockClient>();

        // ✅ RabbitMQ connection
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            HostName = config["RabbitMq:Host"],
            Port = int.Parse(config["RabbitMq:Port"] ?? "5672"),
            UserName = config["RabbitMq:User"],
            Password = config["RabbitMq:Pass"],
            DispatchConsumersAsync = true
        });
        services.AddSingleton<IConnection>(sp =>
            sp.GetRequiredService<IConnectionFactory>().CreateConnection());

        // ✅ Background services
        services.AddHostedService<OutboxPublisher>();
        services.AddHostedService<OrderingSagaConsumer>();

        return services;
    }
}
