//using OrchestratorService.Worker.Messaging;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//public sealed class OrchestratorWorker : BackgroundService
//{
//    private readonly ILogger<OrchestratorWorker> _log;
//    private readonly IConnection _conn;
//    private readonly IConfiguration _cfg;
//    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

//    private string Ex => _cfg["RabbitMq:Exchange"] ?? "order.events";
//    private string QCore => "orchestrator.core_in";         // ⬅️ gộp 3 event vào 1 queue
//    private ushort Prefetch => 50;

//    public OrchestratorWorker(ILogger<OrchestratorWorker> log, IConnection conn, IConfiguration cfg)
//    { _log = log; _conn = conn; _cfg = cfg; }

//    protected override Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        var ch = _conn.CreateModel();
//        ch.ExchangeDeclare(Ex, ExchangeType.Topic, durable: true);

//        // DLX tối thiểu
//        ch.QueueDeclare("orchestrator.dlq", durable: true, exclusive: false, autoDelete: false);
//        var args = new Dictionary<string, object> {
//            { "x-dead-letter-exchange", Ex },
//            { "x-dead-letter-routing-key", "dlq.orchestrator" }
//        };

//        // Queue gộp – bind 3 event
//        ch.QueueDeclare(QCore, durable: true, exclusive: false, autoDelete: false, arguments: args);
//        ch.QueueBind(QCore, Ex, Rk.OrderCreated);
//        ch.QueueBind(QCore, Ex, Rk.PaymentSucceeded);
//        ch.QueueBind(QCore, Ex, Rk.OrderFailed);
//        ch.BasicQos(0, Prefetch, global: false);

//        var consumer = new AsyncEventingBasicConsumer(ch);
//        consumer.Received += async (_, ea) =>
//        {
//            try
//            {
//                switch (ea.RoutingKey)
//                {
//                    case Rk.OrderCreated:
//                        await HandleOrderCreated(ch, ea);
//                        break;

//                    case Rk.PaymentSucceeded:
//                        await HandlePaymentSucceeded(ch, ea);
//                        break;

//                    case Rk.OrderFailed:
//                        await HandleOrderFailed(ch, ea);
//                        break;
//                }

//                ch.BasicAck(ea.DeliveryTag, false);
//            }
//            catch (Exception ex)
//            {
//                _log.LogError(ex, "Handle {rk} failed", ea.RoutingKey);
//                ch.BasicNack(ea.DeliveryTag, multiple: false, requeue: false); // -> DLQ
//            }
//        };

//        ch.BasicConsume(QCore, autoAck: false, consumer);
//        _log.LogInformation("Orchestrator started. Listening: {QCore}", QCore);
//        return Task.CompletedTask;
//    }

//    /* ============ HANDLERS ============ */

//    private Task HandleOrderCreated(IModel ch, BasicDeliverEventArgs ea)
//    {
//        var env = Deserialize<EventEnvelope<OrderCreatedData>>(ea.Body.Span);
//        if (env is null)
//            return Task.CompletedTask;

//        // 🔒 Chốt dữ liệu tối thiểu
//        if (env.OrderId == Guid.Empty || env.Data is null || env.Data.Items is null || env.Data.Items.Count == 0)
//        {
//            _log.LogWarning("order.created payload invalid -> ack and skip. Body={Body}",
//                Encoding.UTF8.GetString(ea.Body.ToArray()));
//            return Task.CompletedTask; // hoặc BasicNack -> DLQ nếu muốn
//        }

//        _log.LogInformation("OrderCreated received: OrderId={OrderId}", env.OrderId);

//        var corrId = env.CorrelationId != Guid.Empty ? env.CorrelationId : Guid.NewGuid();

//        // cmd.inventory.reserve
//        var reserve = new EventEnvelope<CmdInventoryReserve>(
//            Rk.CmdInventoryReserve,
//            corrId,
//            env.OrderId,
//            new CmdInventoryReserve(env.OrderId,
//                env.Data.Items.Select(i => new ReservedItem(i.ProductId, i.Quantity)).ToList()),
//            DateTime.UtcNow
//        );
//        Publish(ch, reserve.EventType, reserve);

//        // cmd.payment.request (nếu bạn vẫn muốn bắn ngay)
//        var grand = env.Data.GrandTotal;
//        var curr = string.IsNullOrWhiteSpace(env.Data.Currency) ? "VND" : env.Data.Currency;
//        var pay = new EventEnvelope<CmdPaymentRequest>(
//            Rk.CmdPaymentRequest, corrId, env.OrderId,
//            new CmdPaymentRequest(env.OrderId, grand, curr), DateTime.UtcNow);
//        Publish(ch, pay.EventType, pay);

//        return Task.CompletedTask;
//    }


//    private Task HandlePaymentSucceeded(IModel ch, BasicDeliverEventArgs ea)
//    {
//        var env = Deserialize<EventEnvelope<PaymentSucceededData>>(ea.Body.Span);
//        if (env is null) return Task.CompletedTask;

//        _log.LogInformation("PaymentSucceeded: OrderId={OrderId}, Txn={Txn}", env.OrderId, env.Data.TxnRef);

//        // Phát order.confirmed (cho OrderingService/Notification)
//        var ok = new EventEnvelope<object>(Rk.OrderConfirmed, env.CorrelationId, env.OrderId, new { }, DateTime.UtcNow);
//        Publish(ch, ok.EventType, ok);

//        // Có thể phát thêm cmd.order.update-status=Paid/Confirmed nếu muốn Ordering update ngay.
//        var up = new EventEnvelope<CmdOrderUpdateStatus>(Rk.CmdOrderUpdateStatus, env.CorrelationId, env.OrderId,
//            new CmdOrderUpdateStatus(env.OrderId, "Paid"), DateTime.UtcNow);
//        Publish(ch, up.EventType, up);

//        return Task.CompletedTask;
//    }

//    private Task HandleOrderFailed(IModel ch, BasicDeliverEventArgs ea)
//    {
//        var env = Deserialize<EventEnvelope<OrderFailedData>>(ea.Body.Span);
//        if (env is null) return Task.CompletedTask;

//        _log.LogWarning("OrderFailed: OrderId={OrderId}, Reason={Reason}", env.OrderId, env.Data.Reason);

//        // Bù trừ: release stock + cập nhật Cancelled
//        var rel = new EventEnvelope<CmdInventoryRelease>(Rk.CmdInventoryRelease, env.CorrelationId, env.OrderId,
//            new CmdInventoryRelease(env.OrderId, new List<ReservedItem>()), DateTime.UtcNow);
//        Publish(ch, rel.EventType, rel);

//        var up = new EventEnvelope<CmdOrderUpdateStatus>(Rk.CmdOrderUpdateStatus, env.CorrelationId, env.OrderId,
//            new CmdOrderUpdateStatus(env.OrderId, "Cancelled"), DateTime.UtcNow);
//        Publish(ch, up.EventType, up);

//        return Task.CompletedTask;
//    }

//    /* ============ HELPERS ============ */

//    private T? Deserialize<T>(ReadOnlySpan<byte> body)
//        => JsonSerializer.Deserialize<T>(body, _json);

//    private void Publish<T>(IModel ch, string routingKey, EventEnvelope<T> env)
//    {
//        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(env, _json));
//        var props = ch.CreateBasicProperties();
//        props.ContentType = "application/json";
//        props.DeliveryMode = 2;
//        ch.BasicPublish(Ex, routingKey, props, body);
//        _log.LogInformation("Published {rk} (order {id})", routingKey, env.OrderId);
//    }
//}
