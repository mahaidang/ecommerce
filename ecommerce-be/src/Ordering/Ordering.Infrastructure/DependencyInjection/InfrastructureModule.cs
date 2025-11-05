using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common;
using Ordering.Infrastructure.Models;



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

        //// ✅ gRPC Client (tới InventoryService)
        //services.AddSingleton(sp =>
        //{
        //    var addr = config["Grpc:InventoryBaseUrl"]!;
        //    var ch = GrpcChannel.ForAddress(addr);
        //    return new InventoryService.Grpc.Inventory.InventoryClient(ch);
        //});
        //services.AddSingleton<IInventoryStockClient, GrpcInventoryStockClient>();

        // ✅ MassTransit (thay cho RabbitMQ thủ công + OutboxPublisher + SagaConsumer)
        services.AddMassTransit(x =>
        {
            // Nếu sau này có consumer (như UpdateOrderStatusConsumer) thì add ở đây
            x.AddConsumers(typeof(InfrastructureModule).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config["RabbitMq:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(config["RabbitMq:User"] ?? "guest");
                    h.Password(config["RabbitMq:Pass"] ?? "guest");
                });

                // Tự động tạo queue, exchange, binding dựa theo conventions
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
