//// OrderingSagaConsumer: Background service lắng nghe các message từ RabbitMQ để xử lý các lệnh liên quan đến đơn hàng.
//// Áp dụng Saga Pattern để đảm bảo tính nhất quán khi cập nhật trạng thái đơn hàng qua các sự kiện.

//using MediatR;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Ordering.Application.Orders.Command;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//namespace Ordering.Infrastructure.Saga;

//// Service chạy nền, nhận message từ RabbitMQ và xử lý các lệnh cập nhật trạng thái đơn hàng.
//public sealed class OrderingSagaConsumer : BackgroundService
//{
//    // Logger để ghi log quá trình xử lý message.
//    private readonly ILogger<OrderingSagaConsumer> _log;
//    // Kết nối RabbitMQ.
//    private readonly IConnection _conn;
//    // Đọc cấu hình từ appsettings.
//    private readonly IConfiguration _cfg;
//    // Factory tạo scope DI cho mỗi message (đảm bảo thread-safe, lấy IMediator từ scope).
//    private readonly IServiceScopeFactory _scopeFactory;
//    // Cấu hình serializer cho JSON.
//    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

//    // Tên exchange RabbitMQ (lấy từ cấu hình).
//    private string Ex => _cfg["RabbitMq:Exchange"] ?? "order.events";

//    // Constructor: inject các dependency.
//    public OrderingSagaConsumer(
//        ILogger<OrderingSagaConsumer> log,
//        IConnection conn,
//        IConfiguration cfg,
//        IServiceScopeFactory scopeFactory)
//    {
//        _log = log; _conn = conn; _cfg = cfg; _scopeFactory = scopeFactory;
//    }

//    // Hàm chính chạy vòng lặp background, đăng ký consumer với RabbitMQ.
//    protected override Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        var ch = _conn.CreateModel();
//        // Khởi tạo exchange và queue nếu chưa có.
//        ch.ExchangeDeclare(Ex, ExchangeType.Topic, durable: true);
//        var q = "ordering.cmd_in";
//        ch.QueueDeclare(q, durable: true, exclusive: false, autoDelete: false);
//        // Bind queue với các routing key cần lắng nghe.
//        ch.QueueBind(q, Ex, "cmd.order.update-status");
//        ch.QueueBind(q, Ex, "order.confirmed");
//        ch.BasicQos(0, 50, false); // Giới hạn số lượng message xử lý đồng thời.

//        var consumer = new AsyncEventingBasicConsumer(ch);
//        // Xử lý khi nhận được message từ RabbitMQ.
//        consumer.Received += async (_, ea) =>
//        {
//            try
//            {
//                var rk = ea.RoutingKey;
//                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
//                _log.LogInformation("Ordering received {rk}: {json}", rk, json);

//                // Mỗi message tạo một scope DI, lấy IMediator từ scope.
//                using var scope = _scopeFactory.CreateScope();
//                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

//                // Xử lý lệnh cập nhật trạng thái đơn hàng.
//                if (rk == "cmd.order.update-status")
//                {
//                    var env = JsonSerializer.Deserialize<EventEnvelope<CmdOrderUpdateStatus>>(json, _json);
//                    if (env is not null)
//                    {
//                        var ok = await mediator.Send(new UpdateStatusCommand(env.OrderId, env.Data.NewStatus), stoppingToken);
//                        _log.LogInformation("UpdateStatus({Status}) -> {Ok}", env.Data.NewStatus, ok);
//                    }
//                }
//                // Xử lý sự kiện đơn hàng đã xác nhận.
//                else if (rk == "order.confirmed")
//                {
//                    var env = JsonSerializer.Deserialize<EventEnvelope<object>>(json, _json);
//                    if (env is not null)
//                    {
//                        var ok = await mediator.Send(new UpdateStatusCommand(env.OrderId, "Confirmed"), stoppingToken);
//                        _log.LogInformation("OrderConfirmed -> UpdateStatus(Confirmed) -> {Ok}", ok);
//                    }
//                }

//                // Xác nhận đã xử lý message thành công.
//                ch.BasicAck(ea.DeliveryTag, false);
//            }
//            catch (Exception ex)
//            {
//                // Nếu lỗi, ghi log và requeue message để retry.
//                _log.LogError(ex, "OrderingSagaConsumer error");
//                ch.BasicNack(ea.DeliveryTag, false, requeue: true);
//            }
//            await Task.CompletedTask;
//        };

//        // Đăng ký consumer với RabbitMQ.
//        ch.BasicConsume(q, autoAck: false, consumer);
//        return Task.CompletedTask;
//    }

//    // Contracts (định nghĩa cấu trúc message đồng bộ với Orchestrator).
//    public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime OccurredAtUtc);
//    public record CmdOrderUpdateStatus(Guid OrderId, string NewStatus);
//}
