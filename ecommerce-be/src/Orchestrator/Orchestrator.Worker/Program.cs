using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchestrator.Worker.Consumers;
using Orchestrator.Worker.Models;
using Orchestrator.Worker.Repositories;
using Shared.Contracts.Events;

var builder = Host.CreateApplicationBuilder(args);

// ✅ Đăng ký DbContext
builder.Services.AddDbContext<SagaDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    // Hoặc test: options.UseInMemoryDatabase("Saga");
});

// ✅ Đăng ký repository
builder.Services.AddScoped<OrderSagaRepository>();

// ✅ Đăng ký MassTransit trực tiếp (thay toàn bộ RabbitMQ.Client thủ công)
builder.Services.AddMassTransit(x =>
{
    // Đăng ký các consumer (như bạn đã tách ra)
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<InventoryEventsConsumer>();
    x.AddConsumer<PaymentEventsConsumer>();
    x.AddConsumer<AdminEventsConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMq:Pass"] ?? "guest");
        });

        // MassTransit tự tạo exchange/queue từ các consumer
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddLogging(c => c.AddConsole());

var app = builder.Build();
await app.RunAsync();

