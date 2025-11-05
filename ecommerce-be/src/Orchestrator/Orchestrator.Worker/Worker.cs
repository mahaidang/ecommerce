//using OrchestratorService.Worker.Messaging;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//namespace OrchestratorService.Worker;


//public sealed class OrchestratorWorker : BackgroundService
//{
//    private readonly ILogger<OrchestratorWorker> _log;
//    private readonly IConnection _conn;
//    private readonly IConfiguration _cfg;
//    private string Ex => _cfg["RabbitMq:Exchange"] ?? "order.events";

//    public OrchestratorWorker(ILogger<OrchestratorWorker> log, IConnection conn, IConfiguration cfg)
//    { _log = log; _conn = conn; _cfg = cfg; }

//    protected override Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        var ch = _conn.CreateModel();
//        ch.ExchangeDeclare(Ex, ExchangeType.Topic, durable: true);

//        // declare queues cho orchestrator (độc lập với queues của service khác)
//        ch.QueueDeclare("orchestrator.order_created", durable: true, exclusive: false, autoDelete: false);
//        ch.QueueBind("orchestrator.order_created", Ex, "order.created");

//        ch.QueueDeclare("orchestrator.inventory_out", durable: true, exclusive: false, autoDelete: false);
//        ch.QueueBind("orchestrator.inventory_out", Ex, "inventory.stock.*"); // reserved/failed

//        ch.QueueDeclare("orchestrator.payment_out", durable: true, exclusive: false, autoDelete: false);
//        ch.QueueBind("orchestrator.payment_out", Ex, "payment.*"); // succeeded/failed

//        // consumer 1: khi có order.created -> yêu cầu reserve stock
//        var c1 = new AsyncEventingBasicConsumer(ch);
//        c1.Received += async (_, ea) =>
//        {
//            try
//            {
//                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
//                _log.LogInformation("order.created: {json}", json);

//                // Deserialize envelope
//                var env = JsonSerializer.Deserialize<EventEnvelope<OrderCreatedData>>(json);
//                if (env is null) { ch.BasicAck(ea.DeliveryTag, false); return; }

//                // Tạo command: cmd.inventory.reserve
//                var cmd = new EventEnvelope<CmdInventoryReserve>(
//                    "cmd.inventory.reserve",
//                    env.CorrelationId != Guid.Empty ? env.CorrelationId : Guid.NewGuid(),
//                    env.OrderId,
//                    new CmdInventoryReserve(env.OrderId,
//                        env.Data.Items.Select(i => new ReservedItem(i.ProductId, i.Quantity)).ToList()),
//                    DateTime.UtcNow // Thêm tham số utcNow vào đây
//                );
//                Publish(ch, Ex, cmd.EventType, cmd);

//                ch.BasicAck(ea.DeliveryTag, false);
//            }
//            catch (Exception ex)
//            {
//                _log.LogError(ex, "Handle order.created failed");
//                ch.BasicNack(ea.DeliveryTag, false, requeue: true);
//            }
//        };
//        ch.BasicConsume("orchestrator.order_created", autoAck: false, c1);

//        // consumer 2: inventory outcomes
//        var c2 = new AsyncEventingBasicConsumer(ch);
//        c2.Received += async (_, ea) =>
//        {
//            var rk = ea.RoutingKey;
//            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
//            _log.LogInformation("inventory event [{rk}]: {json}", rk, json);

//            if (rk == "inventory.stock.reserved")
//            {
//                var env = JsonSerializer.Deserialize<EventEnvelope<InventoryReservedData>>(json);
//                if (env is not null)
//                {
//                    // Gọi thanh toán
//                    var cmd = new EventEnvelope<CmdPaymentRequest>(
//                        "cmd.payment.request",
//                        env.CorrelationId, env.OrderId,
//                        new CmdPaymentRequest(env.OrderId, Amount: 0, Currency: "VND"),
//                        DateTime.UtcNow // Add utcNow argument
//                    );
//                    Publish(ch, Ex, cmd.EventType, cmd);
//                }
//            }
//            else if (rk == "inventory.stock.failed")
//            {
//                var env = JsonSerializer.Deserialize<EventEnvelope<InventoryFailedData>>(json);
//                if (env is not null)
//                {
//                    // Hủy đơn
//                    var cmd = new EventEnvelope<CmdOrderUpdateStatus>(
//                        "cmd.order.update-status",
//                        env.CorrelationId, env.OrderId,
//                        new CmdOrderUpdateStatus(env.OrderId, "Cancelled"),
//                        DateTime.UtcNow // Add utcNow argument
//                    );
//                    Publish(ch, Ex, cmd.EventType, cmd);
//                }
//            }

//            ch.BasicAck(ea.DeliveryTag, false);
//            await Task.CompletedTask;
//        };
//        ch.BasicConsume("orchestrator.inventory_out", autoAck: false, c2);

//        // consumer 3: payment outcomes
//        var c3 = new AsyncEventingBasicConsumer(ch);
//        c3.Received += async (_, ea) =>
//        {
//            var rk = ea.RoutingKey;
//            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
//            _log.LogInformation("payment event [{rk}]: {json}", rk, json);

//            if (rk == "payment.succeeded")
//            {
//                var env = JsonSerializer.Deserialize<EventEnvelope<PaymentSucceededData>>(json);
//                if (env is not null)
//                {
//                    // Đơn xác nhận
//                    var evt = new EventEnvelope<object>(
//                        "order.confirmed", env.CorrelationId, env.OrderId, new { }, DateTime.UtcNow // Add utcNow argument
//                    );
//                    Publish(ch, Ex, evt.EventType, evt);
//                }
//            }
//            else if (rk == "payment.failed")
//            {
//                var env = JsonSerializer.Deserialize<EventEnvelope<PaymentFailedData>>(json);
//                if (env is not null)
//                {
//                    // Bù trừ: release stock + update status cancelled
//                    var rel = new EventEnvelope<CmdInventoryRelease>(
//                        "cmd.inventory.release", env.CorrelationId, env.OrderId,
//                        new CmdInventoryRelease(env.OrderId, new List<ReservedItem>()), DateTime.UtcNow // Thêm tham số utcNow
//                    );
//                    Publish(ch, Ex, rel.EventType, rel);

//                    var up = new EventEnvelope<CmdOrderUpdateStatus>(
//                        "cmd.order.update-status", env.CorrelationId, env.OrderId,
//                        new CmdOrderUpdateStatus(env.OrderId, "Cancelled"), DateTime.UtcNow // Thêm tham số utcNow
//                    );
//                    Publish(ch, Ex, up.EventType, up);
//                }
//            }

//            ch.BasicAck(ea.DeliveryTag, false);
//            await Task.CompletedTask;
//        };
//        ch.BasicConsume("orchestrator.payment_out", autoAck: false, c3);

//        return Task.CompletedTask;
//    }

//    private static void Publish<T>(IModel ch, string exchange, string routingKey, EventEnvelope<T> env)
//    {
//        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(env));
//        var props = ch.CreateBasicProperties();
//        props.ContentType = "application/json";
//        props.DeliveryMode = 2;
//        ch.BasicPublish(exchange, routingKey, props, body);
//    }
//}
