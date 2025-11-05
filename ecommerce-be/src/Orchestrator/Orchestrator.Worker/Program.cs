using MassTransit;
using Orchestrator.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// ✅ Đăng ký MassTransit trực tiếp (thay toàn bộ RabbitMQ.Client thủ công)
builder.Services.AddMassTransit(x =>
{
    // Đăng ký các consumer (như bạn đã tách ra)
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<InventoryEventsConsumer>();
    x.AddConsumer<PaymentEventsConsumer>();

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

